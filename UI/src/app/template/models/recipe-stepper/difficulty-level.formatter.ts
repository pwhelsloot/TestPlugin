import { DinnerPlanDifficultyLevelLookup } from './dinner-plan-difficulty-level-lookup.model';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { DifficultyLevelEnum } from './difficulty-level-enum';

export class DifficultyLevelFormatter {
  static format(difficultyLevels: DinnerPlanDifficultyLevelLookup[]) {
    if (isTruthy(difficultyLevels) && difficultyLevels.length > 0) {
      difficultyLevels.forEach((element) => {
        switch (<DifficultyLevelEnum>element.id) {
          case DifficultyLevelEnum.EASY:
            element.iconClass = 'fal fa-thermometer-quarter fa-2x';
            break;

          case DifficultyLevelEnum.MEDIUM:
            element.iconClass = 'fal fa-thermometer-half fa-2x';
            break;

          case DifficultyLevelEnum.HARD:
            element.iconClass = 'fal fa-thermometer-full fa-2x';
            break;

          default:
            break;
        }
      });
    }
  }
}
