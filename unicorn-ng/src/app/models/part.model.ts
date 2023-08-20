export class Part {
    id: number;
    serialNumber: number;
    dateManufactured: Date;

    constructor(id: number, serialNumber: number, dateManufactured: Date) {
        this.id = id;
        this.serialNumber = serialNumber;
        this.dateManufactured = dateManufactured;
    }
}