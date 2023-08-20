export class Article { 
    id: number;
    serialNumber: number;
    dateManufactured: string;
    price: number;
    action: boolean;

    constructor(id: number, serialNumber: number, dateManufactured: string, price: number, action: boolean) {
        this.id = id;
        this.serialNumber = serialNumber;
        this.dateManufactured = dateManufactured;
        this.price = price;
        this.action = action;
    } 
}