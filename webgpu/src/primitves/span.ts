export class Span {

    constructor(public min: number, public max: number) { }

    length() {
        return this.max - this.min;
    }

    zoomRelative(factor: number, position: number) {
        var zoomPoint = this.min + this.length() * position;
        this.zoom(factor, zoomPoint);
    }

    zoom(factor: number, position: number) {
        var newLeftSide = (position - this.min) * factor;
        var newRightSide = (this.max - position) * factor;
        this.min = position - newLeftSide;
        this.max = position + newRightSide;
    }

    panRelative(pan: number) {
        let p = pan * this.length();
        this.min += p;
        this.max += p;
    }
}