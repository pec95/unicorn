import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { Part } from '../models/part.model';
import { map } from 'rxjs';
import { Car } from '../models/car.model';
import { PartPost } from '../models/partPost.model';

@Injectable({
  providedIn: 'root'
})
export class WarehouseService {
  parts: Part[] = [];
  partsEmpty: Part[] = [];
  partsSubject: BehaviorSubject<Part[]>;

  cars: Car[] = [];
  carsEmpty: Car[] = [];
  carsSubject: BehaviorSubject<Car[]>;

  constructor(private http: HttpClient) { 
    this.partsSubject = new BehaviorSubject(this.partsEmpty);
    this.allParts();

    this.carsSubject = new BehaviorSubject(this.carsEmpty);
    this.allCars();
  }

  private allParts() {
    this.http.get('https://localhost:44378/api/warehouse/allParts')
    .pipe(map(res => {
      let partsObj = Object.create(res);
      for(let key in partsObj) {
        this.parts.push(partsObj[key]);
      }

      return this.parts;
    }))
    .subscribe(res => {
      this.partsSubject.next([...res]);
    });
  }

  getParts() {
    return this.partsSubject;
  }

  private allCars() {
    this.http.get('https://localhost:44378/api/warehouse/getCars')
    .pipe(map(res => {
      let carsObj = Object.create(res);
      for(let key in carsObj) {
        this.cars.push(carsObj[key]);
      }

      return this.cars;
    }))
    .subscribe(res => {
      this.carsSubject.next(res);
    });
  }

  getCars() {
    return this.carsSubject;
  }

  addPart(newPart: PartPost) {
    this.http.post('https://localhost:44378/api/warehouse/addPart', newPart)
      .subscribe(res =>  {
        if(res != false) {
          let obj = Object.create(res);
          this.parts.push(obj);

          this.partsSubject.next(this.parts);
        }
      });
  }

  deletePart(partId: number) {
    this.http.delete(`https://localhost:44378/api/warehouse/deletePart/${partId}`)
      .subscribe(res =>  {
        if(res != false) {
          let index = this.parts.findIndex(p => p.id == res);
          this.parts.splice(index, 1);
  
          this.partsSubject.next(this.parts);
        }
      });
  }

  searchBySerial(serialNumber: number) {
    this.http.get(`https://localhost:44378/api/warehouse/searchSerial/${serialNumber}`)
    .subscribe(res => {
      if(res == true) {
        let part = this.parts.find(p => p.serialNumber == serialNumber);
        if(part != undefined) {
          this.parts = [];
          this.parts.push(part);

          this.partsSubject.next(this.parts);
        }
      }
    });
  }

  searchByDate(date: string) {
    this.http.get(`https://localhost:44378/api/warehouse/searchDate/${date}`)
    .pipe(map(res => {
      let obj = Object.create(res);
      this.parts = [];
      for(let key in obj) {
        this.parts.push(obj[key]);
      }

      return this.parts;
    }))
    .subscribe(res => {
      this.partsSubject.next(res);
    });
  }

  searchByCar(car: string) {
    this.http.get(`https://localhost:44378/api/warehouse/searchCar/${car}`)
    .pipe(map(res => {
      console.log(res);
      
      let obj = Object.create(res);
      console.log(obj);
      this.parts = [];
      for(let key in obj) {
        this.parts.push(obj[key]);
      }

      return this.parts;
    }))
    .subscribe(res => {
      this.partsSubject.next(res);
    });
  }

  reset() {
    this.parts = [];
    this.allParts();
  }
}
