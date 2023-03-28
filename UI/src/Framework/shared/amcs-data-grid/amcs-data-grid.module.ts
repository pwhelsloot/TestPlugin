import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { ModuleWithProviders, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '@shared-module/shared.module';
import { AgGridModule } from 'ag-grid-angular';
import { CsvExportModule, LicenseManager, Module, ModuleRegistry } from 'ag-grid-enterprise';
import { AmcsDataGridComponent } from './amcs-data-grid.component';
import { CheckboxCellEditorComponent } from './cells/checkbox/checkbox-cell-editor/checkbox-cell-editor.component';
import { CheckboxCellRendererComponent } from './cells/checkbox/checkbox-cell-renderer/checkbox-cell-renderer.component';
import { DateCellEditorComponent } from './cells/date/date-cell-editor/date-cell-editor.component';
import { DateCellRendererComponent } from './cells/date/date-cell-renderer/date-cell-renderer.component';
import { NumericCellEditorComponent } from './cells/numeric/numeric-cell-editor/numeric-cell-editor.component';
import { NumericCellRendererComponent } from './cells/numeric/numeric-cell-renderer/numeric-cell-renderer.component';
import { ValidationCellRendererComponent } from './cells/validation/validation-cell-renderer/validation-cell-renderer.component';
import { DataGridLicenseKey } from './data-grid-license-key.model';
import { SingleClickCheckboxCellRendererComponent } from './cells/checkbox/single-click-checkbox-cell-renderer/single-click-checkbox-cell-renderer.component';

const RENDERERS = [CheckboxCellRendererComponent, NumericCellRendererComponent, DateCellRendererComponent, ValidationCellRendererComponent, SingleClickCheckboxCellRendererComponent];
const EDITORS = [CheckboxCellEditorComponent, NumericCellEditorComponent, DateCellEditorComponent];

export const moduleDeclarations = [AmcsDataGridComponent, ...RENDERERS, ...EDITORS];
export const moduleImports = [AgGridModule, FormsModule, CommonModule];
const moduleExports = [AgGridModule, AmcsDataGridComponent, CommonModule];

@NgModule({
  imports: [SharedModule, ...moduleImports],
  declarations: [...moduleDeclarations],
  providers: [DatePipe, DecimalPipe],
  exports: [...moduleExports],
})
export class AmcsDataGridModule {
  /**
   *
   * @param licenseKey
   * @returns
   */
  static forRoot(licenseKey?: DataGridLicenseKey, ...modules: Module[]): ModuleWithProviders<AmcsDataGridModule> {
    if (licenseKey) {
      LicenseManager.setLicenseKey(licenseKey as string);
    }

    ModuleRegistry.registerModules([CsvExportModule, ...modules]);

    return {
      ngModule: AmcsDataGridModule,
    };
  }
}
