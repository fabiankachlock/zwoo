import Logger from '../logging/logImport';
import { Backend, Endpoint } from './apiConfig';
import { BackendErrorAble, parseBackendError } from './errors';

export class ConfigService {
  static async fetchVersion(): Promise<BackendErrorAble<string>> {
    Logger.Api.log(`fetching version`);
    if (process.env.VUE_APP_USE_BACKEND !== 'true') {
      Logger.Api.debug('mocking version response');
      return process.env.VUE_APP_VERSION;
    }

    const req = await fetch(`${Backend.getUrl(Endpoint.Version)}?t=${Date.now()}`);

    if (req.status !== 200) {
      Logger.Api.warn('received erroneous response while fetching version');
      return {
        error: parseBackendError(await req.text())
      };
    }

    return await req.text();
  }

  static async fetchChangelog(version: string): Promise<BackendErrorAble<string>> {
    Logger.Api.log(`fetching changelog for ${version}`);
    if (process.env.VUE_APP_USE_BACKEND !== 'true') {
      Logger.Api.debug('mocking changelog response');
      return `<ul><li><span style="font-weight: bold;">Test<sub>2</sub><sup>2</sup></span></li><li><span style=" font-weight: bold;"><sup>slkdföksdf<span style="text-decoration: underline;">sldömfm</span></sup></span></li><li><span style="  font-weight: bold;"><sup>wlijdfops&lt;dfopls&lt;df</sup>wö<span style="text-decoration: line-through;">oeskr</span></span></li><li><span style="   font-weight: bold;">is<span style="font-style: italic;">pjd</span>f</span>oisdjf<span style="   font-weight: bold;"><br></span></li></ul><ol><li><span style=" font-weight: bold;">slkdfjskldf</span></li><li><span style="  font-weight: bold;">sopakdfüsddf</span></li><li><span style="   font-weight: bold;">wepijfspokdf<br></span></li></ol><p style="margin-left: 160px;"><span style="font-weight: bold;">ykjhdfkjxhdfkjhsfdg</span><br></p><div><br></div><h1><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h1><div><h1 style="text-align: center;"><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h1><div><h1 style="text-align: right;"><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h1><div style="text-align: justify;"><h1><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h1><div><br></div><div><h2><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h2><h2 style=" text-align: center;"><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h2><h2 style=" text-align: right;"><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h2><h2><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h2></div><div><h3><br></h3></div><div><h3><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h3><h3 style=" text-align: center;"><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h3><h3 style=" text-align: right;"><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h3><h3><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h3><div><br></div><div><div><h3><br></h3></div><div><h4><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h4><h4 style="  text-align: center;"><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h4><h4 style="  text-align: right;"><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h4><h4><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h4><div><div><div><h5><br></h5></div><div><h5><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h5><h5 style="   text-align: center;"><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h5><h5 style="   text-align: right;"><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h5><h5><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h5><div><br></div><div><div><div><h3><br></h3></div><div><h6><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h6><div><br></div><div><div><div><h3><br></h3></div><div><p><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></p><p style="   text-align: center;"><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></p><p style="   text-align: right;"><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></p><p><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></p></div></div></div><h6 style="   text-align: center;"><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h6><h6 style="   text-align: right;"><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h6><h6><span style="text-decoration: line-through;"><span style="text-decoration: underline;"><span style="font-style: italic;">1.0.0-beta.4</span></span></span></h6></div></div></div></div></div></div></div></div></div></div></div></div><h1></h1>`;
    }

    const req = await fetch(Backend.getDynamicUrl(Endpoint.Changelog, { version: version }));

    if (req.status !== 200) {
      Logger.Api.warn(`received erroneous response while fetching changelog ${version}`);
      return {
        error: parseBackendError(await req.text())
      };
    }
    return await req.text();
  }
}
