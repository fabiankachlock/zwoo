export interface CSharpExport {
  Test(callback: (n: number) => number): Promise<number>;
}
