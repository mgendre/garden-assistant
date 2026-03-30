import { Component, inject, OnInit, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TranslateModule } from '@ngx-translate/core';
import { WhatsNewEntry } from './whats-new-entry/whats-new-entry';

interface WhatsNewIndexEntry {
  date: string;
  title: string;
  file: string;
}

interface WhatsNewItem {
  date: string;
  title: string;
  content: string;
}

@Component({
  selector: 'app-whats-new',
  standalone: true,
  imports: [TranslateModule, WhatsNewEntry],
  templateUrl: './whats-new.html',
  styleUrl: './whats-new.scss'
})
export class WhatsNew implements OnInit {
  private readonly http = inject(HttpClient);

  readonly loading = signal(true);
  readonly entries = signal<WhatsNewItem[]>([]);

  async ngOnInit(): Promise<void> {
    try {
      const index = await this.fetchIndex();
      const items = await Promise.all(
        index.map(entry => this.fetchEntry(entry))
      );
      this.entries.set(items);
    } finally {
      this.loading.set(false);
    }
  }

  private async fetchIndex(): Promise<WhatsNewIndexEntry[]> {
    const response = await fetch('/changelogs/whats-new.index.json');
    return response.json();
  }

  private async fetchEntry(entry: WhatsNewIndexEntry): Promise<WhatsNewItem> {
    const response = await fetch(`/changelogs/${entry.file}`);
    const raw = await response.text();
    const content = this.stripFrontmatter(raw);
    return { date: entry.date, title: entry.title, content };
  }

  private stripFrontmatter(markdown: string): string {
    const match = markdown.match(/^---\n[\s\S]*?\n---\n/);
    if (match) {
      return markdown.slice(match[0].length).trim();
    }
    return markdown;
  }
}
