import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { Action } from '../models/action.model';
import { Article } from '../models/article.model';
import { ArticlePost } from '../models/articlePost.model';
import { Part } from '../models/part.model';

@Injectable({
  providedIn: 'root'
})
export class SalesService {
  articles: Article[] = [];
  articlesEmpty: Article[] = [];
  articlesSubject: BehaviorSubject<Article[]>;

  parts: Part[] = [];
  partsEmpty: Part[] = [];
  partsSubject: BehaviorSubject<Part[]>;

  constructor(private http: HttpClient) {
    this.articlesSubject = new BehaviorSubject(this.articlesEmpty);
    this.allArticles();

    this.partsSubject = new BehaviorSubject(this.partsEmpty);
    this.partsAvalaible();
  }

  private allArticles() {
    this.http.get('https://localhost:44378/api/sales/allArticles')
    .pipe(map(res => {
      let obj = Object.create(res);
      for(let key in obj) {
        this.articles.push(obj[key]);
      }

      return this.articles;
    }))
    .subscribe(res => {
      this.articlesSubject.next([...res]);
    });
  }

  private partsAvalaible() {
    this.http.get('https://localhost:44378/api/sales/parts')
    .pipe(map(res => {
      let partsResponse = Array.of(res);

      if(partsResponse.length > 0) {
        this.parts = [];
        let obj = Object.create(res);
        
        for(let key in obj) {
          this.parts.push(obj[key]);
        }
      }
    
      return this.parts;
    }))
    .subscribe(res => {
      this.partsSubject.next([...res]);
    });
  }

  getArticles() {
    return this.articlesSubject;
  }

  getParts() {
    return this.partsSubject;
  }
  
  newAction(newAction: Action) {
    this.http.post('https://localhost:44378/api/sales/newAction', newAction)
    .subscribe(res =>  {
      console.log(res);

      if(res == true) {
        this.articles = [];
        this.allArticles();
      }
      else {
        console.log("Error on server");
      }
    });
  }

  deleteArticle(id: number) {
    this.http.delete(`https://localhost:44378/api/sales/deleteArticle/${id}`)
    .subscribe(res =>  {
      if(res != false) {
        let index = this.articles.findIndex(p => p.id == res);
        
        this.articles.splice(index, 1);
  
        this.articlesSubject.next(this.articles);
          
        this.partsAvalaible();
      }
    });
  }

  newArticle(newArticle: ArticlePost) {
    this.http.post('https://localhost:44378/api/sales/newArticle', newArticle)
    .subscribe(res =>  {
      if(res != false) {       
        let obj = Object.create(res);
        this.articles.push(obj);

        this.articlesSubject.next(this.articles);

        this.partsAvalaible();
      }
    });
  }

  changeBasePrice(id: number, newPrice: number) {
    let newPriceObj = { newPrice: newPrice }
    this.http.put(`https://localhost:44378/api/sales/changeArticlePrice/${id}`, newPriceObj)
    .subscribe(res =>  {
      if(res != false) {
        this.articles = [];

        this.allArticles();
      }
    });
  }
}
