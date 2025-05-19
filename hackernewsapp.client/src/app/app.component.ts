import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

interface HackerNewsItem {
  id: number;
  deleted: boolean;
  type: string;
  by: string;
  time: number;
  text: string;
  dead: boolean;
  parent: number;
  poll: number;
  kids: number[];
  url: string;
  score: number;
  title: string;
  parts: number[];
  descendants: number;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  public stories: HackerNewsItem[] = [];
  public pageNum: number = 0;
  public pageSize: number = 25;
  public query:string = '';

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getLatestStories();
  }

  getLatestStories() {
    this.http.get<HackerNewsItem[]>(`https://localhost:7176/latest?pageNum=${this.pageNum}&pageSize=${this.pageSize}`).subscribe(
      (result) => {
        this.stories = result;
      },
      (error) => {
        console.error(error);
      }
    );
  }

  searchStories() {
    this.http.get<HackerNewsItem[]>(`https://localhost:7176/search?query=${this.query}&pageNum=${this.pageNum}&pageSize=${this.pageSize}`).subscribe(
      (result) => {
        this.stories = result;
      },
      (error) => {
        console.error(error);
      }
    );
  }

  viewNextPage() {
    this.pageNum++;
    this.stories = [];

    if (this.query == '')
      this.getLatestStories();
    else
      this.searchStories();
  }

  viewPrevPage() {
    this.pageNum--;
    this.stories = [];

    if (this.query == '')
      this.getLatestStories();
    else
      this.searchStories();
  }

  title = 'hackernewsapp.client';
}
