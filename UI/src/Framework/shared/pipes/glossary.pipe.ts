import { Pipe, PipeTransform } from '@angular/core';
import { GlossaryService } from '@core-module/services/glossary/glossary.service';

@Pipe({
  name: 'glossary',
})
export class GlossaryPipe implements PipeTransform {
  constructor(private readonly glossaryService: GlossaryService) {}
  transform(value: string): string {
    return this.glossaryService.getGlossaryTranslation(value);
  }
}
