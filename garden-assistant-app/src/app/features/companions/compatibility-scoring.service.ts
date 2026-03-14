import { Injectable } from '@angular/core';
import {
  AssociationEffect,
  AssociationMechanism,
  ConfidenceLevel,
  DistanceEffect,
  PlantAssociationDto
} from '../../api/garden-assistant-api';

export type Verdict = 'INCOMPATIBLE' | 'Harmful' | 'Neutral' | 'Beneficial';

export interface CompatibilityResult {
  score: number;
  verdict: Verdict;
  blocked: boolean;
  blockingAssociations: PlantAssociationDto[];
}

const MECHANISM_MULTIPLIER: Partial<Record<AssociationMechanism, number>> = {
  [AssociationMechanism.RootAllelopathy]:    2.0,
  [AssociationMechanism.AerialRepulsion]:    1.5,
  [AssociationMechanism.NitrogenFixation]:   1.5,
  [AssociationMechanism.PredatorAttraction]: 1.4,
  [AssociationMechanism.PollinatorAttraction]:1.2,
  [AssociationMechanism.DynamicAccumulation]:1.2,
  [AssociationMechanism.SoilCover]:          1.2,
  [AssociationMechanism.PhysicalSupport]:    1.0,
  [AssociationMechanism.OlfactoryConfusion]: 0.8,
  [AssociationMechanism.TrapCrop]:           0.8,
};

const DISTANCE_MULTIPLIER: Record<DistanceEffect, number> = {
  [DistanceEffect.Contact]: 1.5,
  [DistanceEffect.Short]:   1.2,
  [DistanceEffect.Medium]:  1.0,
  [DistanceEffect.Field]:   0.6,
};

const CONFIDENCE_MULTIPLIER: Record<ConfidenceLevel, number> = {
  [ConfidenceLevel.PeerReviewed]:  1.0,
  [ConfidenceLevel.FieldObserved]: 0.75,
  [ConfidenceLevel.Anecdotal]:     0.5,
};

@Injectable({ providedIn: 'root' })
export class CompatibilityScoringService {

  compute(associations: PlantAssociationDto[]): CompatibilityResult {
    const blocking = associations.filter(a =>
      a.effect === AssociationEffect.Harmful &&
      a.mechanism === AssociationMechanism.RootAllelopathy &&
      (a.confidenceLevel === ConfidenceLevel.PeerReviewed || a.confidenceLevel === ConfidenceLevel.FieldObserved) &&
      (a.distanceEffect === DistanceEffect.Contact || a.distanceEffect === DistanceEffect.Short)
    );

    if (blocking.length > 0) {
      return { score: -Infinity, verdict: 'INCOMPATIBLE', blocked: true, blockingAssociations: blocking };
    }

    let total = 0;
    for (const assoc of associations) {
      const base = assoc.effect === AssociationEffect.Beneficial ? 1.0
                 : assoc.effect === AssociationEffect.Harmful    ? -1.5
                 : 0.0;

      const mMult = assoc.mechanism !== undefined ? (MECHANISM_MULTIPLIER[assoc.mechanism] ?? 1.0) : 1.0;
      const dMult = assoc.distanceEffect !== undefined ? (DISTANCE_MULTIPLIER[assoc.distanceEffect] ?? 1.0) : 1.0;
      const cMult = assoc.confidenceLevel !== undefined ? (CONFIDENCE_MULTIPLIER[assoc.confidenceLevel] ?? 0.5) : 0.5;

      total += base * mMult * dMult * cMult;
    }

    const score = Math.round(total * 10) / 10;
    const verdict: Verdict = score >= 1.0  ? 'Beneficial'
                           : score <= -1.0 ? 'Harmful'
                           : 'Neutral';

    return { score, verdict, blocked: false, blockingAssociations: [] };
  }
}
