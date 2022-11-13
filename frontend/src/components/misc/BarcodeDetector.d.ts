// TODO: ts definitions
declare global {
	// eslint-disable-next-line no-var
	var BarcodeDetector: {
		detect(src: Blob | HTMLCanvasElement | HTMLImageElement | HTMLVideoElement | ImageBitmap | ImageData | SVGImageElement): Promise<
			{
				boundingBox: {
					x: number;
					y: number;
					width: number;
					height: number;
					top: number;
					right: number;
					bottom: number;
					left: number;
				};
				cornerPoints: [
					{
						x: number;
						y: number;
					}
				];
				format: string;
				rawValue: string;
			}[]
		>;
		getSupportedFormats(): Promise<string[]>;
		new (): typeof BarcodeDetector;
	};
}
