import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LoggerService {
  log(msg: string) {
    console.log(msg);
  }

  logObj(msg: string, obj: any) {
    console.log(msg, obj);
  }

  error(msg: string) {
    console.error(msg);
  }
}
