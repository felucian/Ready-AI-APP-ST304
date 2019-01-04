import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'book',
    templateUrl: './book.component.html',
    styleUrls: ['./book.component.css']
})

export class BookComponent {
    public books: Book[] | undefined;
    public reviews: BookReviews[] | undefined;
    public currentBookId: number | undefined;
    public http: Http;
    public baseUrl: string;
    public reviewLoadingFailure: boolean = false;
    public bookLoadingFailure: boolean = false;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.http = http;
        this.baseUrl = baseUrl;
        this.http.get(this.baseUrl + 'api/BookData/BookList').subscribe(books => {
            this.books = books.json() as Book[];
            this.bookLoadingFailure = false;

            if (this.books != undefined && this.books.length > 0) {
                this.currentBookId = 1;
                this.getReviews(this.currentBookId);
            }

        }, error => {
            console.error(error);
            this.bookLoadingFailure = true;
        });
    }

    public updateReviews(newBookId: number) {
        this.reviews = undefined;
        this.currentBookId = newBookId;
        this.getReviews(this.currentBookId);
    }

    private getReviews(bookId: number) {
        this.http.get(this.baseUrl + 'api/BookData/Reviews/' + bookId).subscribe(reviews => {
            this.reviewLoadingFailure = false;
            this.reviews = reviews.json() as BookReviews[];
            console.log(this.reviews);
        }, error => {
            console.error(error);
            this.reviewLoadingFailure = true;
        });
    }
}

interface Book {
    Id: number;
    Title: string;
    Author: string;
    Year: number;
    Price: number;
}

interface BookReviews {
    Id: number;
    BookId: number;
    Author: string;
    Title: string;
    Description: string;
    Rating: number;
}

