import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'breakword'
})

export class BreakWordPipe implements PipeTransform {

  /*
    this is intended for use in cases where the css setting overflow-wrap: break-word is correctly breaking the text at word boundaries,
    but not breaking the text within a long word where that is required to fit within the container.

    so this will break any words exceeding the character limit by inserting newline characters.

    the css class whte-space: pre-line can then be used to correctly break on these newline literals in the output html.
  */
  transform(value: string, args: string[]): string {

    if (!value) {
      return value;
    }

    let charLimit = 20;

    if (args) {
      if (args.length > 0) {
        charLimit = parseInt(args[0], 10);
      }
    }

    const words = value.match(/\S+\s*/g);
    const processedWords: string[] = [];

    for (const word of words) {
      let currentWord = word;

      if (currentWord.length > charLimit) {
        const wordParts: string[] = [];

        while (currentWord.length > charLimit) {
          wordParts.push(currentWord.substring(0, charLimit));
          currentWord = currentWord.substring(charLimit);
        }

        if (currentWord.length > 0) {
          wordParts.push(currentWord);
        }

        processedWords.push(wordParts.join('\n'));
      } else {
        processedWords.push(currentWord);
      }
    }

    return processedWords.join(' ');
  }
}
