import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { BehaviorSubject, Subscription } from 'rxjs';
import { Action } from '../models/action.model';
import { Article } from '../models/article.model';
import { ArticlePost } from '../models/articlePost.model';
import { Part } from '../models/part.model';
import { AuthorizationService } from '../services/authorization.service';
import { SalesService } from '../services/sales.service';
import { WarehouseService } from '../services/warehouse.service';

@Component({
  selector: 'app-sales',
  templateUrl: './sales.component.html',
  styleUrls: ['./sales.component.css']
})
export class SalesComponent implements OnInit {

  validatedArticle: boolean = true;
  validatedAction: boolean = true;
  actionAdd: boolean = false;
  articleAdd: boolean = false;
  newPrice: number[] = [];

  user: string = '';
  formCreateArticle: FormGroup;
  formCreateAction: FormGroup;

  parts: Part[] = [];
  partsEmpty: Part[] = [];
  partsSubject: BehaviorSubject<Part[]>;
  subscriptionParts: Subscription = new Subscription();

  articles: Article[] = [];
  articlesEmpty: Article[] = [];
  articlesSubject: BehaviorSubject<Article[]>;
  subscriptionArticles: Subscription = new Subscription();

  articlesNoAction: Article[] = [];

  constructor(private salesService: SalesService, private userService: AuthorizationService, private fb: FormBuilder) {
    this.partsSubject = new BehaviorSubject(this.partsEmpty);
    this.articlesSubject = new BehaviorSubject(this.articlesEmpty);
    
    this.formCreateArticle = this.fb.group({
      'part': new FormControl('', [Validators.required]),
      'price': new FormControl('', [Validators.required])
    });

    this.formCreateAction = this.fb.group({
      'startDate': new FormControl('', [Validators.required]),
      'endDate': new FormControl('', [Validators.required]),
      'articles': new FormControl('', [Validators.required]),
      'discount': new FormControl('', [Validators.required])
    });
  }

  ngOnInit(): void {
    this.partsSubject = this.salesService.getParts();
    this.subscriptionParts = this.partsSubject.subscribe(res => { 
      this.parts = [...res];
    });

    this.articlesSubject = this.salesService.getArticles();
    this.subscriptionArticles = this.articlesSubject.subscribe(res => {
      this.articles = [...res];

      this.newPrice = [];
      this.articles.forEach(a => {
        this.newPrice.push(0);
      });

      console.log(this.newPrice);
      

      this.articlesNoAction = this.articles.filter(a => a.action != true);
      console.log(this.articles, this.articlesNoAction);
    });

    this.user = this.userService.getUser();
  }

  articleBtn() {
    this.articleAdd = !this.articleAdd;
  }

  actionBtn() {
    this.actionAdd = !this.actionAdd;
  }

  createArticle() {
    let part = this.formCreateArticle.controls['part'];
    let price = this.formCreateArticle.controls['price'];

    if(part.invalid) {
      this.validatedArticle = false;
    }
    else if(price.invalid) {
      this.validatedArticle = false;
    }
    else {
      let article = new ArticlePost(part.value, price.value);
      this.salesService.newArticle(article);

      this.formCreateArticle.reset();
      this.articleBtn();
      this.validatedArticle = true;
    }
  }

  createAction() {
    let start = this.formCreateAction.controls['startDate'];
    let end = this.formCreateAction.controls['endDate'];
    let articles = this.formCreateAction.controls['articles'];
    let discount = this.formCreateAction.controls['discount'];

    let today = Date.now();
    let startDate = Date.parse(start.value);
    let endDate = Date.parse(end.value);

    if(start.invalid) {
      this.validatedAction = false;
    }
    else if(end.invalid) {
      this.validatedAction = false;
    }
    else if(articles.invalid) {
      this.validatedAction = false;
    }
    else if(discount.invalid) {
      this.validatedAction = false;
    }
    else {
      if(startDate > endDate) {
        this.validatedAction = false;
      }
      else if(endDate < today) {
        this.validatedAction = false;
      }
      else {
        let action = new Action(start.value, end.value, articles.value, discount.value);

        this.salesService.newAction(action);

        this.formCreateAction.reset();

        this.actionBtn();
        this.validatedAction = true;
      }
    }

  }

  changePrice(articleId: number, newPriceIndex: number) {
    let newPrice = this.newPrice[newPriceIndex];

    let article = this.articles.find(a => a.id == articleId);

    if(article != undefined) {
      if(newPrice != article.price) this.salesService.changeBasePrice(articleId, newPrice);
    }
  }

  deleteArticle(articleId: number) {
    this.salesService.deleteArticle(articleId);
  }

  logout() {
    this.userService.logout();
  }

  ngOnDestroy(): void {
    this.subscriptionParts.unsubscribe();
    this.subscriptionArticles.unsubscribe();
  }
}
