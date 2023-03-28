import { environment } from '@environments/environment';

export class AmcsBeep {

  static beep() {
    const audio = new Audio();
    const prefix = environment.applicationURLPrefix.replace(/^\/|\/$/g, '');
    audio.src = `/${prefix}/Framework/assets/sounds/beep.wav`;
    audio.load();
    audio.play();
  }
}
