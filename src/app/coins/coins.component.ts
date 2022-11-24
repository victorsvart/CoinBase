import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-coins',
  templateUrl: './coins.component.html',
  styleUrls: ['./coins.component.scss']
})
export class CoinsComponent implements OnInit {


  public Coin: any;
  public Converters: any;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.getCoin();
  }

  public getCoin(): void {
    this.http.get('https://localhost:44345/Coin/CoinList').subscribe(
      response => this.Coin = response,
      error => console.log(error),
    );
  }
}
