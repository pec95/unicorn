export class Action { 
    startDate: string;
    endDate: string;
    articles: number[];
    discount: number;

    constructor(startDate: string, endDate: string, articles: number[], discount: number) {
        this.startDate = startDate;
        this.endDate = endDate;
        this.articles = articles;
        this.discount = discount;
    } 
}