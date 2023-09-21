import { DataGenerator } from "./dataGenerator";
import { UIInteraction } from "./uiInteraction";
import { HairRenderer } from "./hairRenderer";
import { FastLineRenderer } from "./fastLineRenderer";

console.log("Initiating");
const canvas = document.querySelector("canvas")!;
let data = DataGenerator.createMultiLineData(500, 500);
let renderer = new FastLineRenderer(canvas, data[0]);
//let renderer = new HairRenderer(canvas, data[0]);
renderer.xRange = data[1];
renderer.yRange = data[2];
await renderer.initialize();
let ui = new UIInteraction(renderer);
renderer.render();