export class PartPost {
    serialNumber: number;
    dateManufactured: Date;
    carId: number;

    constructor(serialNumber: number, dateManufactured: Date, carId: number) {
        this.serialNumber = serialNumber;
        this.dateManufactured = dateManufactured;
        this.carId = carId;
    }
}