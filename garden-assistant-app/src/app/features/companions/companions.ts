import { Component, computed, inject, OnInit, signal } from '@angular/core';
import {
  AssociationEffect,
  AssociationMechanism,
  ConfidenceLevel,
  DistanceEffect,
  PlantAssociationDto,
} from '../../api/garden-assistant-api';
import { CompatibilityScoringService } from './compatibility-scoring.service';
import { CompanionSearchService } from './companion-search.service';

const MECHANISM_LABELS: Record<AssociationMechanism, string> = {
  [AssociationMechanism.OlfactoryConfusion]:  'Odeur qui déroute les insectes',
  [AssociationMechanism.PollinatorAttraction]:'Attire les abeilles et papillons',
  [AssociationMechanism.TrapCrop]:            'Plante sacrificielle',
  [AssociationMechanism.RootAllelopathy]:     'Toxines racinaires',
  [AssociationMechanism.AerialRepulsion]:     'Arômes répulsifs',
  [AssociationMechanism.NitrogenFixation]:    'Enrichit le sol en azote',
  [AssociationMechanism.PredatorAttraction]:  'Attire les insectes utiles',
  [AssociationMechanism.PhysicalSupport]:     'Tuteur naturel',
  [AssociationMechanism.SoilCover]:           'Couvre-sol vivant',
  [AssociationMechanism.DynamicAccumulation]: 'Remonte les minéraux',
};

const MECHANISM_FALLBACK_NOTES: Record<AssociationMechanism, string> = {
  [AssociationMechanism.OlfactoryConfusion]:  'Les huiles essentielles de cette plante perturbent la détection des ravageurs.',
  [AssociationMechanism.PollinatorAttraction]:'Les fleurs de cette plante attirent abeilles et papillons au profit des voisines.',
  [AssociationMechanism.TrapCrop]:            'Plante sacrificielle : surveillez et retirez-la avant que les ravageurs ne se dispersent.',
  [AssociationMechanism.RootAllelopathy]:     'Les exsudats racinaires de cette plante inhibent la croissance de ses voisines.',
  [AssociationMechanism.AerialRepulsion]:     'Les composés volatils de cette plante repoussent certains insectes nuisibles.',
  [AssociationMechanism.NitrogenFixation]:    `Ses racines abritent des bactéries qui fixent l'azote atmosphérique au profit des voisines.`,
  [AssociationMechanism.PredatorAttraction]:  'Cette plante attire coccinelles, syrphes et parasitoïdes — des alliés contre les ravageurs.',
  [AssociationMechanism.PhysicalSupport]:     'Cette plante sert de tuteur naturel ou de support physique à sa voisine.',
  [AssociationMechanism.SoilCover]:           `Son feuillage dense protège le sol, conserve l'humidité et étouffe les adventices.`,
  [AssociationMechanism.DynamicAccumulation]: 'Ses racines profondes remontent les minéraux en surface, enrichissant le sol pour les voisines.',
};

const DISTANCE_LABELS: Record<DistanceEffect, string> = {
  [DistanceEffect.Contact]: 'Au contact (< 15 cm)',
  [DistanceEffect.Short]:   'Proximité (15–50 cm)',
  [DistanceEffect.Medium]:  'Même carré (50 cm–2 m)',
  [DistanceEffect.Field]:   'Effet de jardin (> 2 m)',
};

const CONFIDENCE_LABELS: Record<ConfidenceLevel, string> = {
  [ConfidenceLevel.Anecdotal]:    'Tradition populaire',
  [ConfidenceLevel.FieldObserved]:'Observé par des jardiniers',
  [ConfidenceLevel.PeerReviewed]: 'Confirmé scientifiquement',
};

const CONFIDENCE_TOOLTIP: Record<ConfidenceLevel, string> = {
  [ConfidenceLevel.Anecdotal]:    'Rapporté sans vérification systématique',
  [ConfidenceLevel.FieldObserved]:'Documenté par des jardiniers expérimentés',
  [ConfidenceLevel.PeerReviewed]: 'Confirmé par des études scientifiques',
};

@Component({
  selector: 'app-companions',
  standalone: true,
  providers: [CompanionSearchService],
  template: `
    <div class="min-h-screen bg-[#f8f4ef]">
      <header class="mb-8">
        <h1 class="font-['DM_Serif_Display'] text-3xl text-[#1e3d1e]">Associations de plantes</h1>
        <p class="mt-1 text-sm text-[#6b4226]">Sélectionnez 2 à 5 plantes pour analyser leur compatibilité</p>
      </header>

      @if (service.error()) {
        <div role="alert" class="mb-4 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          {{ service.error() }}
        </div>
      }

      <div class="flex flex-col gap-6 lg:flex-row">

        <!-- Plant list -->
        <aside class="flex w-full flex-col gap-3 lg:w-80 lg:shrink-0">
          <label for="plant-search" class="sr-only">Rechercher une plante</label>
          <input
            id="plant-search"
            type="text"
            placeholder="Rechercher une plante..."
            [value]="searchTerm()"
            (input)="searchTerm.set($any($event.target).value)"
            class="w-full rounded-lg border border-[#c8b99a] bg-white px-4 py-2.5 text-sm text-[#1e3d1e] placeholder-[#a89070] shadow-sm focus:outline-none focus:ring-2 focus:ring-[#4a7c3f]"
          />

          @if (service.loading() && service.selectedPlants().length === 0) {
            <div role="status" class="flex justify-center py-8">
              <div class="h-8 w-8 animate-spin rounded-full border-4 border-[#4a7c3f] border-t-transparent"></div>
              <span class="sr-only">Chargement des plantes…</span>
            </div>
          }

          <ul class="flex max-h-[60vh] flex-col gap-1.5 overflow-y-auto pr-1" aria-label="Liste des plantes">
            @for (plant of filteredPlants(); track plant.id) {
              <li>
                <button
                  (click)="service.togglePlant(plant)"
                  [attr.aria-pressed]="service.isSelected(plant)"
                  [attr.aria-label]="plant.name + (plant.scientificName ? ' — ' + plant.scientificName : '')"
                  class="group w-full rounded-lg border px-4 py-2.5 text-left text-sm transition-all duration-150"
                  [class]="service.isSelected(plant)
                    ? 'border-[#4a7c3f] bg-[#e8f0e8] ring-1 ring-[#4a7c3f]'
                    : 'border-[#e0d6cc] bg-white hover:border-[#4a7c3f]/40 hover:bg-[#f0ebe4]'"
                >
                  <div class="flex items-center gap-2">
                    <span aria-hidden="true"
                      class="flex h-4 w-4 shrink-0 items-center justify-center rounded border transition-all"
                      [class]="service.isSelected(plant)
                        ? 'border-[#4a7c3f] bg-[#4a7c3f] text-white'
                        : 'border-[#c8b99a] bg-white'">
                      @if (service.isSelected(plant)) {
                        <svg class="h-3 w-3" viewBox="0 0 12 12" fill="none">
                          <path d="M2 6l3 3 5-5" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                        </svg>
                      }
                    </span>
                    <div class="min-w-0">
                      <span class="block truncate font-medium text-[#1e3d1e]">{{ plant.name }}</span>
                      @if (plant.scientificName) {
                        <span class="block truncate text-xs italic text-[#6b4226]">{{ plant.scientificName }}</span>
                      }
                    </div>
                  </div>
                </button>
              </li>
            }
            @if (filteredPlants().length === 0 && !service.loading()) {
              <li class="py-6 text-center text-sm text-[#6b4226]">Aucune plante trouvée</li>
            }
          </ul>
        </aside>

        <!-- Results -->
        <main class="min-w-0 flex-1">

          <!-- Selected plants chips -->
          @if (service.selectedPlants().length > 0) {
            <div class="mb-5 flex flex-wrap gap-2" aria-label="Plantes sélectionnées">
              @for (plant of service.selectedPlants(); track plant.id) {
                <span class="inline-flex items-center gap-1.5 rounded-full bg-[#1e3d1e] pl-3 pr-1 py-1 text-sm font-medium text-white">
                  {{ plant.name }}
                  <button
                    (click)="service.togglePlant(plant)"
                    [attr.aria-label]="'Retirer ' + plant.name"
                    class="flex items-center justify-center rounded-full p-1.5 -mr-0.5 hover:bg-white/20 transition-colors focus:outline-none focus:ring-2 focus:ring-white/50">
                    <svg aria-hidden="true" class="h-3 w-3" viewBox="0 0 12 12" fill="none">
                      <path d="M2 2l8 8M10 2l-8 8" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"/>
                    </svg>
                  </button>
                </span>
              }
            </div>
          }

          <!-- Empty state: no plants selected -->
          @if (service.selectedPlants().length === 0) {
            <div class="flex flex-col items-center justify-center rounded-xl border-2 border-dashed border-[#c8b99a] bg-white py-24 text-center">
              <span aria-hidden="true" class="mb-4 text-5xl">🌿</span>
              <p class="text-base font-medium text-[#1e3d1e]">Sélectionnez au moins 2 plantes</p>
              <p class="mt-1 text-sm text-[#6b4226]">L'outil analysera toutes les associations entre elles</p>
            </div>
          }

          <!-- Need one more plant -->
          @if (service.selectedPlants().length === 1) {
            <div class="flex flex-col items-center justify-center rounded-xl border-2 border-dashed border-[#4a7c3f]/40 bg-[#e8f0e8]/50 py-16 text-center">
              <span aria-hidden="true" class="mb-3 text-4xl">➕</span>
              <p class="text-sm font-medium text-[#1e3d1e]">Ajoutez une deuxième plante pour voir les associations</p>
            </div>
          }

          <!-- Loading -->
          @if (service.loading() && service.selectedPlants().length >= 2) {
            <div role="status" class="flex justify-center py-16">
              <div class="h-10 w-10 animate-spin rounded-full border-4 border-[#4a7c3f] border-t-transparent"></div>
              <span class="sr-only">Analyse des associations en cours…</span>
            </div>
          }

          <!-- Results -->
          @if (!service.loading() && service.selectedPlants().length >= 2) {

            <div class="mb-6">

              <!-- Hard block — primary message first -->
              @if (compatibility().blocked) {
                <div role="alert" class="mb-4 rounded-xl border-2 border-red-600 bg-red-50 px-5 py-4">
                  <p class="font-bold text-red-700 text-base">⛔ Association incompatible</p>
                  @for (b of compatibility().blockingAssociations; track b.id) {
                    <div class="mt-2 text-sm font-medium text-red-700">
                      <strong>{{ plantName(b.sourcePlantId) }}</strong>
                      <span class="mx-1">inhibe</span>
                      <strong>{{ plantName(b.targetPlantId) }}</strong>
                      @if (b.notes) { — {{ b.notes }} }
                      @else { — {{ mechanismFallbackNote(b.mechanism) }} }
                    </div>
                  }
                  <p class="mt-3 text-sm text-red-600 border-t border-red-200 pt-3">
                    Cette combinaison est déconseillée même si d'autres associations sont bénéfiques.
                  </p>
                </div>
              }

              <!-- Verdict banner -->
              @if (!compatibility().blocked) {
                <div aria-live="polite" class="mb-4 flex items-center gap-4 rounded-xl px-5 py-4"
                  [class]="verdictBannerClass()">
                  <span aria-hidden="true" class="text-2xl">{{ verdictEmoji() }}</span>
                  <div>
                    <p class="font-bold">{{ verdictLabel() }}</p>
                    <p class="text-sm opacity-75 mt-0.5">{{ verdictDescription() }}</p>
                  </div>
                </div>
              }

              <!-- Raw counts -->
              <div class="grid grid-cols-3 gap-3">
                <div class="rounded-xl border border-green-200 bg-green-50 px-4 py-3 text-center">
                  <span class="block text-xl font-bold text-green-700">{{ service.score().beneficial }}</span>
                  <span class="block text-xs font-medium text-green-600">Bénéfiques</span>
                </div>
                <div class="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-center">
                  <span class="block text-xl font-bold text-red-700">{{ service.score().harmful }}</span>
                  <span class="block text-xs font-medium text-red-600">Nuisibles</span>
                </div>
                <div class="rounded-xl border border-gray-200 bg-gray-50 px-4 py-3 text-center">
                  <span class="block text-xl font-bold text-gray-600">{{ service.score().neutral }}</span>
                  <span class="block text-xs font-medium text-gray-500">Neutres</span>
                </div>
              </div>
            </div>

            @if (service.associations().length === 0) {
              <div class="rounded-xl border border-[#e0d6cc] bg-white px-8 py-12 text-center">
                <p class="text-sm text-[#6b4226]">Aucune association répertoriée entre ces plantes.</p>
              </div>
            } @else {
              <ul class="flex flex-col gap-3">
                @for (assoc of service.associations(); track assoc.id) {
                  <li class="rounded-xl border bg-white px-5 py-4 shadow-sm transition-shadow hover:shadow-md"
                    [class]="effectBorderClass(assoc.effect)">
                    <div class="flex flex-wrap items-start justify-between gap-3">
                      <div class="flex items-center gap-2">
                        <span [attr.aria-label]="effectLabel(assoc.effect)" class="text-lg">{{ effectEmoji(assoc.effect) }}</span>
                        <div>
                          <span class="font-semibold text-[#1e3d1e]">{{ plantName(assoc.sourcePlantId) }}</span>
                          <span aria-hidden="true" class="mx-2 text-[#6b4226]">→</span>
                          <span class="sr-only">agit sur</span>
                          <span class="font-semibold text-[#1e3d1e]">{{ plantName(assoc.targetPlantId) }}</span>
                        </div>
                      </div>
                      <div class="flex flex-wrap gap-2">
                        <span class="inline-flex items-center rounded-full px-3 py-0.5 text-xs font-medium text-white"
                          [class]="mechanismChipClass(assoc.mechanism)">
                          {{ mechanismLabel(assoc.mechanism) }}
                        </span>
                        <span class="inline-flex items-center rounded-full border border-[#c0d8bc] bg-[#e8f0e8] px-3 py-0.5 text-xs font-medium text-[#1e3d1e]"
                          [title]="distanceTooltip(assoc.distanceEffect)">
                          {{ distanceLabel(assoc.distanceEffect) }}
                        </span>
                        <span class="inline-flex items-center rounded-full px-3 py-0.5 text-xs font-medium"
                          [class]="confidenceClass(assoc.confidenceLevel)"
                          [title]="confidenceTooltip(assoc.confidenceLevel)">
                          {{ confidenceLabel(assoc.confidenceLevel) }}
                        </span>
                      </div>
                    </div>
                    <p class="mt-3 border-t border-[#f0ebe4] pt-3 text-sm text-[#6b4226]">
                      {{ assoc.notes || mechanismFallbackNote(assoc.mechanism) }}
                    </p>
                  </li>
                }
              </ul>
            }
          }

        </main>
      </div>
    </div>
  `
})
export class CompanionsComponent implements OnInit {
  readonly service = inject(CompanionSearchService);
  readonly scoring = inject(CompatibilityScoringService);
  readonly searchTerm = signal('');

  readonly compatibility = computed(() => this.scoring.compute(this.service.associations()));

  readonly filteredPlants = computed(() => {
    const term = this.searchTerm().toLowerCase().trim();
    if (!term) return this.service.plants();
    return this.service.plants().filter(p =>
      p.name?.toLowerCase().includes(term) ||
      p.scientificName?.toLowerCase().includes(term)
    );
  });

  ngOnInit(): void {
    this.service.loadPlants();
  }

  plantName(id: string | undefined): string {
    return this.service.plants().find(p => p.id === id)?.name ?? 'Plante inconnue';
  }

  verdictLabel(): string {
    switch (this.compatibility().verdict) {
      case 'Beneficial':   return 'Association favorable';
      case 'Harmful':      return 'Association déconseillée';
      case 'Neutral':      return 'Association neutre';
      case 'INCOMPATIBLE': return 'Incompatible';
    }
  }

  verdictDescription(): string {
    switch (this.compatibility().verdict) {
      case 'Beneficial': return `Ces plantes s'entraident activement. Association recommandée.`;
      case 'Harmful':    return 'Ces plantes se perturbent mutuellement. À éviter si possible.';
      case 'Neutral':    return 'Aucune interaction significative documentée. Cohabitation possible.';
      default:           return '';
    }
  }

  verdictEmoji(): string {
    switch (this.compatibility().verdict) {
      case 'Beneficial': return '🌿';
      case 'Harmful':    return '⚠️';
      case 'Neutral':    return '⚖️';
      default:           return '🚫';
    }
  }

  verdictBannerClass(): string {
    switch (this.compatibility().verdict) {
      case 'Beneficial': return 'border border-green-200 bg-green-50 text-green-800';
      case 'Harmful':    return 'border border-orange-200 bg-orange-50 text-orange-800';
      default:           return 'border border-gray-200 bg-gray-50 text-gray-700';
    }
  }

  mechanismLabel(m: AssociationMechanism | undefined): string {
    return m !== undefined ? MECHANISM_LABELS[m] ?? '' : '';
  }

  mechanismFallbackNote(m: AssociationMechanism | undefined): string {
    return m !== undefined ? MECHANISM_FALLBACK_NOTES[m] ?? '' : '';
  }

  mechanismChipClass(m: AssociationMechanism | undefined): string {
    return m === AssociationMechanism.TrapCrop
      ? 'bg-amber-500'
      : 'bg-[#4a7c3f]';
  }

  distanceLabel(d: DistanceEffect | undefined): string {
    return d !== undefined ? DISTANCE_LABELS[d] ?? '' : '';
  }

  distanceTooltip(d: DistanceEffect | undefined): string {
    if (d === DistanceEffect.Contact) return 'Effet maximal au contact direct des racines ou feuilles';
    if (d === DistanceEffect.Short)   return 'Effet dans un rayon de 15 à 50 cm';
    if (d === DistanceEffect.Medium)  return 'Effet dans un même carré ou plate-bande';
    if (d === DistanceEffect.Field)   return `Effet à l'échelle du jardin entier`;
    return '';
  }

  confidenceLabel(level: ConfidenceLevel | undefined): string {
    return level !== undefined ? CONFIDENCE_LABELS[level] ?? '' : '';
  }

  confidenceTooltip(level: ConfidenceLevel | undefined): string {
    return level !== undefined ? CONFIDENCE_TOOLTIP[level] ?? '' : '';
  }

  confidenceClass(level: ConfidenceLevel | undefined): string {
    if (level === ConfidenceLevel.Anecdotal)    return 'bg-gray-100 text-gray-600 border border-gray-200';
    if (level === ConfidenceLevel.FieldObserved) return 'bg-amber-100 text-amber-700 border border-amber-200';
    if (level === ConfidenceLevel.PeerReviewed)  return 'bg-green-100 text-green-700 border border-green-200';
    return 'bg-gray-100 text-gray-600';
  }

  effectEmoji(effect: AssociationEffect | undefined): string {
    if (effect === AssociationEffect.Beneficial) return '✅';
    if (effect === AssociationEffect.Harmful)    return '⚠️';
    return '➖';
  }

  effectLabel(effect: AssociationEffect | undefined): string {
    if (effect === AssociationEffect.Beneficial) return 'Bénéfique';
    if (effect === AssociationEffect.Harmful)    return 'Nuisible';
    return 'Neutre';
  }

  effectBorderClass(effect: AssociationEffect | undefined): string {
    if (effect === AssociationEffect.Beneficial) return 'border-green-200';
    if (effect === AssociationEffect.Harmful)    return 'border-red-200';
    return 'border-[#e0d6cc]';
  }
}
