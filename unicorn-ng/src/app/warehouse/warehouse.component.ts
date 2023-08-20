import { Component, OnInit } from '@angular/core';
import { Form, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { Car } from '../models/car.model';
import { Part } from '../models/part.model';
import { PartPost } from '../models/partPost.model';
import { AuthorizationService } from '../services/authorization.service';
import { WarehouseService } from '../services/warehouse.service';

@Component({
  selector: 'app-warehouse',
  templateUrl: './warehouse.component.html',
  styleUrls: ['./warehouse.component.css']
})
export class WarehouseComponent implements OnInit {

  user: string = '';
  formCreate: FormGroup;
  formSearch: FormGroup;

  validated: boolean = true;
  parts: Part[] = [];
  partsEmpty: Part[] = [];
  partsSubject: BehaviorSubject<Part[]>;
  subscriptionParts: Subscription = new Subscription();

  cars: Car[] = [];
  carsEmpty: Car[] = [];
  carsSubject: BehaviorSubject<Car[]>;
  subscriptionCars: Subscription = new Subscription();

  constructor(private warehouseService: WarehouseService, private userService: AuthorizationService, private fb: FormBuilder) { 
    this.partsSubject = new BehaviorSubject(this.partsEmpty);
    this.carsSubject = new BehaviorSubject(this.carsEmpty);

    this.formCreate = this.fb.group({
      'serial': new FormControl('', [Validators.required]),
      'date': new FormControl('', [Validators.required]),
      'car': new FormControl('', [Validators.required])
    });

    this.formSearch = this.fb.group({
      'serialS': new FormControl(''),
      'dateS': new FormControl(''),
      'carS': new FormControl('')
    });
  }

  ngOnInit(): void {
    this.partsSubject = this.warehouseService.getParts();
    this.subscriptionParts = this.partsSubject.subscribe(res => {   
      this.parts = [...res];
    });

    this.carsSubject = this.warehouseService.getCars();
    this.subscriptionCars = this.carsSubject.subscribe(res => {
      this.cars = [...res];
    });

    this.user = this.userService.getUser();
  }

  createPart() {
    let serial = this.formCreate.controls['serial'];
    let date = this.formCreate.controls['date'];
    let car = this.formCreate.controls['car'];

    if(serial.invalid) {
      this.validated = false;
    }
    else if(date.invalid) {
      this.validated = false;
    }
    else if(car.invalid) {
      this.validated = false;
    }
    else {
      let part = new PartPost(serial.value, date.value, car.value);
      this.warehouseService.addPart(part);

      this.validated = true;
    }
  }

  deletePart(id: number) {
    this.warehouseService.deletePart(id);
  }

  searchSerial() {
    let serial = this.formSearch.controls['serialS'];

    if(typeof(serial.value) === 'number') {
      this.warehouseService.searchBySerial(serial.value);
    }
  }

  searchDate() {
    let date = this.formSearch.controls['dateS'];
    if(date.value != '') {
      this.warehouseService.searchByDate(date.value);
    }
  }

  searchCar() {
    let car = this.formSearch.controls['carS'];
    if(car.value != '') {
      this.warehouseService.searchByCar(car.value);
    }
  }

  resetSearch() {
    this.warehouseService.reset();
  }

  logout() {
    this.userService.logout();
  }

  ngOnDestroy(): void {
    this.subscriptionParts.unsubscribe();
    this.subscriptionCars.unsubscribe();
  }
}
