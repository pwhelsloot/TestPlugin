/* eslint-disable max-classes-per-file */
import { DragDropModule } from '@angular/cdk/drag-drop';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
// eslint-disable-next-line
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSortModule } from '@angular/material/sort';
import { MatStepperModule } from '@angular/material/stepper';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { RouterModule } from '@angular/router';
import { LeafletModule } from '@asymmetrik/ngx-leaflet';
import { LeafletDrawModule } from '@asymmetrik/ngx-leaflet-draw';
import { LeafletMarkerClusterModule } from '@asymmetrik/ngx-leaflet-markercluster';
import { NgSelectModule } from '@ng-select/ng-select';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { AmcsFilterInlineComponent } from '@shared-module/components/amcs-filter-inline/amcs-filter-inline.component';
import { AmcsFilterComponent } from '@shared-module/components/amcs-filter/amcs-filter.component';
import { AmcsInnerTileComponent } from '@shared-module/components/amcs-inner-tile/amcs-inner-tile.component';
import { AmcsRadioControlRegistryDirective } from '@shared-module/components/amcs-radio-input/amcs-radio-control-registry.directive';
import { AmcsTimepickerExtendedComponent } from '@shared-module/components/amcs-timepicker-extended/amcs-timepicker-extended.component';
import { ClickOutsideDirective } from '@shared-module/directives/click-outside.directive';
import { InstrumentationDirective } from '@shared-module/directives/instrumentation-directive';
import { InstrumentationListenDirective } from '@shared-module/directives/instrumentation-listen-directive';
import { TakeFocusDirective } from '@shared-module/directives/take-focus.directive';
import { BreakWordPipe } from '@shared-module/pipes/break-word.pipe';
import { GlossaryPipe } from '@shared-module/pipes/glossary.pipe';
import { LocalizedDatePipe } from '@translate/localized-datePipe';
import { MultiTranslateHttpLoader } from '@translate/multi-translation-file-loader';
import { ObjectToDatePipe } from '@translate/object-to-date.pipe';
import { ZXingScannerModule } from '@zxing/ngx-scanner';
import { NgxCurrencyModule } from 'amcs-ngx-currency';
import { CalendarModule, DateAdapter } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';
import { JwBootstrapSwitchNg2Module } from 'jw-bootstrap-switch-ng2';
import { ChartsModule } from 'ng2-charts';
import { AccordionModule } from 'ngx-bootstrap/accordion';
import { AlertModule } from 'ngx-bootstrap/alert';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { CarouselModule } from 'ngx-bootstrap/carousel';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ModalModule } from 'ngx-bootstrap/modal';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { PopoverModule } from 'ngx-bootstrap/popover';
import { ProgressbarModule } from 'ngx-bootstrap/progressbar';
import { RatingModule } from 'ngx-bootstrap/rating';
import { SortableModule } from 'ngx-bootstrap/sortable';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { TimepickerModule } from 'ngx-bootstrap/timepicker';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { TypeaheadModule } from 'ngx-bootstrap/typeahead';
import { ClipboardModule } from 'ngx-clipboard';
import { DropzoneModule } from 'ngx-dropzone-wrapper';
import { ImageCropperModule } from 'ngx-image-cropper';
import { AmcsContactComponent } from './amcs-contact/amcs-contact.component';
import { AmcsLeafletMapComponent } from './amcs-leaflet-map/amcs-leaflet-map.component';
import { AmcsNavBarActionsComponent } from './amcs-navbar-actions/amcs-navbar-actions.component';
import { AmcsAddressValidatorComponent } from './components/amcs-address-validator/amcs-address-validator.component';
import { AmcsAlertComponent } from './components/amcs-alert/amcs-alert.component';
import { AmcsAppliedFilterListComponent } from './components/amcs-applied-filter-list/amcs-applied-filter-list.component';
import { AmcsBarcodeScannerComponent } from './components/amcs-barcode-scanner/amcs-barcode-scanner.component';
import { AmcsBrowserEditorTileComponent } from './components/amcs-browser-editor-tile/amcs-browser-editor-tile.component';
import { AmcsBrowserTileDeprecatedComponent } from './components/amcs-browser-tile-deprecated/amcs-browser-tile-deprecated.component';
import { AmcsBrowserTileComponent } from './components/amcs-browser-tile/amcs-browser-tile.component';
import { AmcsButtonLoginAzureComponent } from './components/amcs-button-login-azure/amcs-button-login-azure.component';
import { AmcsButtonLoginGoogleComponent } from './components/amcs-button-login-google/amcs-button-login-google.component';
import { AmcsButtonLoginOktaComponent } from './components/amcs-button-login-okta/amcs-button-login-okta.component';
import { AmcsButtonComponent } from './components/amcs-button/amcs-button.component';
import { AmcsCalendarAllDayEventComponent } from './components/amcs-calendar/amcs-calendar-all-day-event/amcs-calendar-all-day-event.component';
import { AmcsCalendarDesktopCellComponent } from './components/amcs-calendar/amcs-calendar-desktop-cell/amcs-calendar-desktop-cell.component';
import { AmcsCalendarWeekEventComponent } from './components/amcs-calendar/amcs-calendar-week-event/amcs-calendar-week-event.component';
import { AmcsCalendarComponent } from './components/amcs-calendar/amcs-calendar.component';
import { CalendarOpenDayEventsComponent } from './components/amcs-calendar/calendar-open-day-events/calendar-open-day-events.component';
import { AmcsCardComponent } from './components/amcs-card/amcs-card.component';
import { AmcsCarouselComponent } from './components/amcs-carousel/amcs-carousel.component';
import { AmcsChangeLogComponent } from './components/amcs-change-log/amcs-change-log.component';
import { AmcsChartComponent } from './components/amcs-chart/amcs-chart.component';
import { AmcsChildTableComponent } from './components/amcs-child-table/amcs-child-table.component';
import { AmcsCollapseComponent } from './components/amcs-collapse/amcs-collapse.component';
import { AmcsCommentComponent } from './components/amcs-comment/amcs-comment.component';
import { AmcsComponentFilterComponent } from './components/amcs-component-filter/amcs-component-filter.component';
import { ComponentFilterEditorComponent } from './components/amcs-component-filter/component-filter-editor/component-filter-editor.component';
import { AmcsContainerTileComponent } from './components/amcs-container-tile/amcs-container-tile.component';
import { AmcsCostTemplateSelectorComponent } from './components/amcs-cost-template-selector/amcs-cost-template-selector.component';
import { CostTemplateDetailsComponent } from './components/amcs-cost-template-selector/cost-template-details/cost-template-details.component';
import { AmcsCustomInnerTileComponent } from './components/amcs-custom-inner-tile/amcs-custom-inner-tile.component';
import { AmcsCustomTileComponent } from './components/amcs-custom-tile/amcs-custom-tile.component';
import { AmcsDataInputGridColumnDirective } from './components/amcs-data-input-grid/amcs-data-input-grid-column.directive';
import { AmcsDataInputGridComponent } from './components/amcs-data-input-grid/amcs-data-input-grid.component';
import { AmcsDateRangeFilterComponent } from './components/amcs-date-range-filter/amcs-date-range-filter.component';
import { AmcsDatepickerComponent } from './components/amcs-datepicker/amcs-datepicker.component';
import { AmcsDaterangepickerComponent } from './components/amcs-daterangepicker/amcs-daterangepicker.component';
import { AmcsDefaultActionSelectorComponent } from './components/amcs-default-action-selector/amcs-default-action-selector.component';
import { DefaultActionDetailsComponent } from './components/amcs-default-action-selector/default-action-details/default-action-details.component';
import { DirectDebitEditorComponent } from './components/amcs-direct-debit-editor/amcs-direct-debit-editor.component';
import { AmcsDragDropComponent } from './components/amcs-drag-drop/amcs-drag-drop.component';
import { AmcsDropdownMenuComponent } from './components/amcs-dropdown-menu/amcs-dropdown-menu.component';
import { AmcsDropdownSelectComponent } from './components/amcs-dropdown-select/amcs-dropdown-select.component';
import { AmcsDropdownComponent } from './components/amcs-dropdown/amcs-dropdown.component';
import { AmcsDynamicTileComponent } from './components/amcs-dynamic-tile/amcs-dynamic-tile.component';
import { AmcsExpandCollapseComponent } from './components/amcs-expand-collapse/amcs-expand-collapse.component';
import { AmcsExternalPaymentButtonComponent } from './components/amcs-external-payment-button/amcs-external-payment-button.component';
import { AmcsExternalPaymentComponent } from './components/amcs-external-payment/amcs-external-payment.component';
import { AmcsFieldDefinitionTableComponent } from './components/amcs-field-definition-table/amcs-field-definition-table.component';
import { AmcsFilterMobileComponent } from './components/amcs-filter-mobile/amcs-filter-mobile.component';
import { AmcsFilterSingleSelectComponent } from './components/amcs-filter-single-select/amcs-filter-single-select.component';
import { AmcsFormGroupComponent } from './components/amcs-form-group/amcs-form-group.component';
import { AmcsFormLabelComponent } from './components/amcs-form-label/amcs-form-label.component';
import { AmcsFormModalComponent } from './components/amcs-form-modal/amcs-form-modal.component';
import { AmcsGlobalSearchComponent } from './components/amcs-global-search/amcs-global-search.component';
import { AmcsGridActionColumnHeaderComponent } from './components/amcs-grid-action-column/amcs-grid-action-column-header/amcs-grid-action-column-header.component';
import { AmcsGridActionColumnComponent } from './components/amcs-grid-action-column/amcs-grid-action-column.component';
import { AmcsGridColumnCustomiserComponent } from './components/amcs-grid-column-customiser/amcs-grid-column-customiser.component';
import { AmcsGridFormSimpleComponent } from './components/amcs-grid-form-simple/amcs-grid-form-simple.component';
import { AmcsGridFormColumnDirective } from './components/amcs-grid-form/amcs-grid-form-column.directive';
import { AmcsGridFormComponent } from './components/amcs-grid-form/amcs-grid-form.component';
import { AmcsGridWrapperComponent } from './components/amcs-grid-wrapper/amcs-grid-wrapper.component';
import { AmcsGridComponent } from './components/amcs-grid/amcs-grid.component';
import { GridEnumerationPipe } from './components/amcs-grid/grid-enumeration.pipe';
import { AmcsHighlightIconComponent } from './components/amcs-highlight-icon/amcs-highlight-icon.component';
import { AmcsInnerContainerGroupComponent } from './components/amcs-inner-container-group/amcs-inner-container-group.component';
import { AmcsInnerContainerComponent } from './components/amcs-inner-container/amcs-inner-container.component';
import { AmcsInputDefinitionComponent } from './components/amcs-input-definition/amcs-input-definition.component';
import { AmcsInputComponent } from './components/amcs-input/amcs-input.component';
import { AmcsIsStickyDirective } from './components/amcs-is-sticky/amcs-is-sticky.directive';
import { AmcsLabelDefinitionComponent } from './components/amcs-label-definition/amcs-label-definition.component';
import { AmcsLeafletMapDrawComponent } from './components/amcs-leaflet-map-draw/amcs-leaflet-map-draw.component';
import { AmcsListSelectWithSearchComponent } from './components/amcs-list-select-with-search/amcs-list-select-with-search.component';
import { AmcsListViewComponent } from './components/amcs-list-view/amcs-list-view.component';
import { AmcsLoadingViewComponent } from './components/amcs-loading-view/amcs-loading-view.component';
import { AmcsMenuSelectorComponent } from './components/amcs-menu-selector/amcs-menu-selector.component';
import { AmcsMobileActionButtonsComponent } from './components/amcs-mobile-action-buttons/amcs-mobile-action-buttons.component';
import { AmcsMobileHeaderComponent } from './components/amcs-mobile-header/amcs-mobile-header.component';
import { AmcsMobileLayoutComponent } from './components/amcs-mobile-layout/amcs-mobile-layout.component';
import { AmcsMobileSummaryComponent } from './components/amcs-mobile-summary/amcs-mobile-summary.component';
import { AmcsModalLookupSelectComponent } from './components/amcs-modal-lookup-select/amcs-modal-lookup-select.component';
import { AmcsModalSelectorComponent } from './components/amcs-modal-selector/amcs-modal-selector.component';
import { ModalDetailsComponent } from './components/amcs-modal-selector/modal-selector/modal-details.component';
import { AmcsModalComponent } from './components/amcs-modal/amcs-modal.component';
import { AmcsMultiSelectWithSearchComponent } from './components/amcs-multi-select-with-search/amcs-multi-select-with-search.component';
import { AmcsMultiSelectComponent } from './components/amcs-multi-select/amcs-multi-select.component';
import { AmcsMultiTileSelectorComponent } from './components/amcs-multi-tile-selector/amcs-multi-tile-selector.component';
import { AmcsNavbarComponent } from './components/amcs-navbar/amcs-navbar.component';
import { AmcsNumericalInputComponent } from './components/amcs-numerical-input/amcs-numerical-input.component';
import { AmcsPagerComponent } from './components/amcs-pager/amcs-pager.component';
import { AmcsParentCardComponent } from './components/amcs-parent-card/amcs-parent-card.component';
import { AmcsPictureViewerComponent } from './components/amcs-picture-viewer/amcs-picture-viewer.component';
import { AmcsProgressbarComponent } from './components/amcs-progressbar/amcs-progressbar.component';
import { AmcsQuantitySelectorComponent } from './components/amcs-quantity-selector/amcs-quantity-selector.component';
import { AmcsRadioInputComponent } from './components/amcs-radio-input/amcs-radio-input.component';
import { AmcsRadioTileComponent } from './components/amcs-radio-tile/amcs-radio-tile.component';
import { AmcsReportTileComponent } from './components/amcs-report-tile/amcs-report-tile.component';
import { AmcsSaveButtonComponent } from './components/amcs-save-button/amcs-save-button.component';
import { AmcsSavingComponent } from './components/amcs-saving/amcs-saving.component';
import { ScheduleAdvancedEditorComponent } from './components/amcs-scheduler/schedule-editor/schedule-advanced-editor/schedule-advanced-editor.component';
import { ScheduleDailyEditorComponent } from './components/amcs-scheduler/schedule-editor/schedule-daily-editor/schedule-daily-editor.component';
import { ScheduleEditorComponent } from './components/amcs-scheduler/schedule-editor/schedule-editor.component';
import { ScheduleMonthlyEditorComponent } from './components/amcs-scheduler/schedule-editor/schedule-monthly-editor/schedule-monthly-editor.component';
import { ScheduleWeeklyEditorComponent } from './components/amcs-scheduler/schedule-editor/schedule-weekly-editor/schedule-weekly-editor.component';
import { AmcsSearchPortletComponent } from './components/amcs-search-portlet/amcs-search-portlet.component';
import { AmcsSelectAllCheckboxComponent } from './components/amcs-select-all-checkbox/amcs-select-all-checkbox';
import { AmcsSelectCardComponent } from './components/amcs-select-card/amcs-select-card.component';
import { AmcsSelectCompactTypeaheadComponent } from './components/amcs-select-compact-typeahead/amcs-select-compact-typeahead.component';
import { AmcsSelectDefinitionDeprecatedComponent } from './components/amcs-select-definition-deprecated/amcs-select-definition-deprecated.component';
import { AmcsSelectDefinitionComponent } from './components/amcs-select-definition/amcs-select-definition.component';
import { AmcsSelectDeprecatedComponent } from './components/amcs-select-deprecated/amcs-select-deprecated.component';
import { AmcsSelectTextLabelComponent } from './components/amcs-select-text-label/amcs-select-text-label.component';
import { AmcsSelectTypeaheadComponent } from './components/amcs-select-typeahead/amcs-select-typeahead.component';
import { AmcsSelectWithSearchComponent } from './components/amcs-select-with-search/amcs-select-with-search.component';
import { AmcsSelectComponent } from './components/amcs-select/amcs-select.component';
import { AmcsSelectorGridComponent } from './components/amcs-selector-grid/amcs-selector-grid.component';
import { AmcsSignaturePadComponent } from './components/amcs-signature-pad/amcs-signature-pad.component';
import { AmcsSplitDropdownButtonComponent } from './components/amcs-split-dropdown-button/amcs-split-dropdown-button.component';
import { AmcsStatusBadgeComponent } from './components/amcs-status-badge/amcs-status-badge.component';
import { AmcsStepComponent } from './components/amcs-step/amcs-step.component';
import { AmcsDesktopStepperComponent } from './components/amcs-stepper/amcs-desktop-stepper/amcs-desktop-stepper.component';
import { AmcsMobileStepperComponent } from './components/amcs-stepper/amcs-mobile-stepper/amcs-mobile-stepper.component';
import { AmcsStepperComponent } from './components/amcs-stepper/amcs-stepper.component';
import { AmcsSwipeOptionsComponent } from './components/amcs-swipe-options/amcs-swipe-options.component';
import { AmcsSwitchComponent } from './components/amcs-switch/amcs-switch.component';
import { AmcsTabControlComponent } from './components/amcs-tab-control/amcs-tab-control.component';
import { AmcsTabComponent } from './components/amcs-tab/amcs-tab.component';
import { AmcsTableComponent } from './components/amcs-table/amcs-table.component';
import { AmcsTextAreaComponent } from './components/amcs-text-area/amcs-text-area.component';
import { AmcsTimepickerDeprecatedComponent } from './components/amcs-timepicker-deprecated/amcs-timepicker-deprecated.component';
import { AmcsTimepickerComponent } from './components/amcs-timepicker/amcs-timepicker.component';
import { AmcsTooltipOnTruncateComponent } from './components/amcs-tooltip-on-truncate/amcs-tooltip-on-truncate.component';
import { AmcsTypeaheadComponent } from './components/amcs-typeahead/amcs-typeahead.component';
import { AmcsWorkCentreTaskTileComponent } from './components/amcs-workcentre-task-tile/amcs-workcentre-task-tile.component';
import { DashboardTileComponent } from './components/dashboard-tile/dashboard-tile.component';
import { AmcsBrowserGridEditorLayoutComponent } from './components/layouts/amcs-browser-grid-editor-layout/amcs-browser-grid-editor-layout.component';
import { AmcsBrowserGridLayoutComponent } from './components/layouts/amcs-browser-grid-layout/amcs-browser-grid-layout.component';
import { AmcsColumnComponent } from './components/layouts/amcs-column/amcs-column.component';
import { AmcsFormActionsComponent } from './components/layouts/amcs-form-actions/amcs-form-actions.component';
import { AmcsFormTileComponent } from './components/layouts/amcs-form-tile/amcs-form-tile.component';
import { AmcsFormComponent } from './components/layouts/amcs-form/amcs-form.component';
import { AmcsRowComponent } from './components/layouts/amcs-row/amcs-row.component';
import { ErrorModalComponent } from './components/layouts/error-modal/error-modal.component';
import { PersonalAccessTokenBrowserComponent } from './components/personal-access-token-browser/personal-access-token-browser.component';
import { PersonalAccessTokenSetupComponent } from './components/personal-access-token-setup/personal-access-token-setup.component';
import { AutomationLocatorDirective } from './directives/automation-locator.directive';
import { FocusOnErrorDirective } from './directives/focus-on-error.directive';
import { FormSavingDirective } from './directives/form-saving.directive';
import { ImageViewerDirective } from './directives/image-viewer.directive';
import { HeaderCommunicationsComponent } from './header/header-communications/header-communications.component';
import { HeaderDesktopComponent } from './header/header-desktop/header-desktop.component';
import { HeaderMenuComponent } from './header/header-menu/header-menu.component';
import { HeaderMobileComponent } from './header/header-mobile/header-mobile.component';
import { HeaderNotificationsComponent } from './header/header-notifications/header-notifications.component';
import { HeaderComponent } from './header/header.component';
import { ImageManipulationComponent } from './image-manipulation/image-manipulation/image-manipulation.component';
import { IndexComponent } from './index/index.component';
import { PageLayoutComponent } from './page-layout/page-layout.component';
import { AbsoluteValuePipe } from './pipes/absolute-value.pipe';
import { CurrencyFormatPipe } from './pipes/currency-format.pipe';
import { KeyMaskPipe } from './pipes/key-mask.pipe';
import { MinusToParenthesis } from './pipes/minus-to-parenthesis.pipe';
import { NumberFormatPipe } from './pipes/number-format.pipe';
import { PluraliseItemCountPipe } from './pipes/pluralise-item-count.pipe';
import { TaxRateFormatPipe } from './pipes/tax-rate-format.pipe';

export function createTranslateLoader(http: HttpClient) {
  return new MultiTranslateHttpLoader(http, [
    { prefix: './assets/i18n/shared/', suffix: '.json' },
  ]);
}

export class SharedModuleConstants {
  static AngularModules = [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    FlexLayoutModule,
  ];

  static AngularMatModules = [
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatTabsModule,
    MatNativeDateModule,
    MatExpansionModule,
    MatSidenavModule,
    MatSnackBarModule,
    MatButtonModule,
    MatListModule,
    MatSelectModule,
    MatChipsModule,
    MatIconModule,
    MatStepperModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatAutocompleteModule,
    MatMenuModule,
    DragDropModule
  ];

  static NgXBootstrapModules = [
    AccordionModule.forRoot(),
    AlertModule.forRoot(),
    ButtonsModule.forRoot(),
    CarouselModule.forRoot(),
    CollapseModule.forRoot(),
    BsDatepickerModule.forRoot(),
    BsDropdownModule.forRoot(),
    ModalModule.forRoot(),
    PaginationModule.forRoot(),
    PopoverModule.forRoot(),
    ProgressbarModule.forRoot(),
    RatingModule.forRoot(),
    SortableModule.forRoot(),
    TabsModule.forRoot(),
    TimepickerModule.forRoot(),
    TooltipModule.forRoot(),
    TypeaheadModule.forRoot(),
    JwBootstrapSwitchNg2Module
  ];

  static NgXBootstrapExportModules = [
    AccordionModule,
    AlertModule,
    ButtonsModule,
    CarouselModule,
    CollapseModule,
    BsDatepickerModule,
    BsDropdownModule,
    ModalModule,
    PaginationModule,
    PopoverModule,
    ProgressbarModule,
    RatingModule,
    SortableModule,
    TabsModule,
    TimepickerModule,
    TooltipModule,
    TypeaheadModule,
    JwBootstrapSwitchNg2Module
  ];

  static AmcsPresentationWrappers = [
    AmcsColumnComponent,
    AmcsRowComponent,
    AmcsFormComponent,
    AmcsFormActionsComponent,
    AmcsGridActionColumnComponent,
    AmcsGridColumnCustomiserComponent,
    AmcsFormModalComponent,
    AmcsDatepickerComponent,
    AmcsDaterangepickerComponent,
    AmcsTimepickerComponent,
    AmcsTimepickerExtendedComponent,
    AmcsSwitchComponent,
    AmcsButtonComponent,
    AmcsButtonLoginGoogleComponent,
    AmcsButtonLoginOktaComponent,
    AmcsButtonLoginAzureComponent,
    AmcsAlertComponent,
    AmcsDropdownComponent,
    AmcsDropdownMenuComponent,
    AmcsDropdownSelectComponent,
    AmcsInputComponent,
    AmcsQuantitySelectorComponent,
    AmcsCollapseComponent,
    AmcsProgressbarComponent,
    AmcsTypeaheadComponent,
    AmcsSelectComponent,
    AmcsSelectDeprecatedComponent,
    AmcsCalendarComponent,
    AmcsGlobalSearchComponent,
    AmcsTextAreaComponent,
    AmcsStepperComponent,
    AmcsStepComponent,
    AmcsFilterSingleSelectComponent,
    AmcsModalComponent,
    AmcsDynamicTileComponent,
    AmcsSearchPortletComponent,
    AmcsExpandCollapseComponent,
    AmcsTableComponent,
    AmcsChildTableComponent,
    AmcsRadioInputComponent,
    AmcsRadioControlRegistryDirective,
    AmcsInputDefinitionComponent,
    AmcsSelectDefinitionDeprecatedComponent,
    AmcsLabelDefinitionComponent,
    AmcsFieldDefinitionTableComponent,
    AmcsAddressValidatorComponent,
    AmcsTabComponent,
    AmcsTabControlComponent,
    AmcsSaveButtonComponent,
    AmcsSelectorGridComponent,
    AmcsReportTileComponent,
    AmcsLoadingViewComponent,
    AmcsPagerComponent,
    AmcsSelectAllCheckboxComponent,
    AmcsMultiSelectComponent,
    AmcsSelectDefinitionComponent,
    AmcsSelectTypeaheadComponent,
    AmcsSelectCompactTypeaheadComponent,
    AmcsChartComponent,
    AmcsGridFormComponent,
    AmcsGridFormSimpleComponent,
    AmcsDataInputGridComponent,
    AmcsNumericalInputComponent,
    AmcsHighlightIconComponent,
    AmcsSavingComponent,
    AmcsCardComponent,
    AmcsParentCardComponent,
    AmcsTooltipOnTruncateComponent,
    AmcsListViewComponent,
    AmcsIsStickyDirective,
    AmcsDefaultActionSelectorComponent,
    AmcsSelectWithSearchComponent,
    AmcsInnerContainerComponent,
    AmcsInnerContainerGroupComponent,
    AmcsMultiSelectWithSearchComponent,
    AmcsRadioTileComponent,
    AmcsMenuSelectorComponent,
    AmcsWorkCentreTaskTileComponent,
    AmcsNavBarActionsComponent,
    AmcsCostTemplateSelectorComponent,
    AmcsLeafletMapDrawComponent,
    AmcsExternalPaymentComponent,
    AmcsExternalPaymentButtonComponent,
    AmcsListSelectWithSearchComponent,
    AmcsMultiTileSelectorComponent,
    DirectDebitEditorComponent,
    AmcsModalSelectorComponent,
    AmcsSelectTextLabelComponent,
    AmcsFormTileComponent,
    AmcsSplitDropdownButtonComponent,
    AmcsSwipeOptionsComponent,
    AmcsCommentComponent,
    AmcsSelectCardComponent,
    AmcsMobileHeaderComponent,
    AmcsMobileLayoutComponent,
    AmcsBrowserGridLayoutComponent,
    ScheduleEditorComponent,
    ScheduleAdvancedEditorComponent,
    ScheduleWeeklyEditorComponent,
    ScheduleMonthlyEditorComponent,
    ScheduleDailyEditorComponent,
    AmcsStatusBadgeComponent,
  ];

  static LayoutComponents = [
    HeaderComponent,
    PageLayoutComponent,
    DashboardTileComponent,
    AmcsBrowserTileComponent,
    AmcsInnerTileComponent,
    AmcsCustomTileComponent,
    HeaderCommunicationsComponent,
    HeaderNotificationsComponent,
    AmcsGridComponent,
    GridEnumerationPipe,
    AmcsCustomInnerTileComponent,
    AmcsBrowserGridLayoutComponent,
    AmcsBrowserTileDeprecatedComponent,
    AmcsGridWrapperComponent,
    PersonalAccessTokenSetupComponent,
    PersonalAccessTokenBrowserComponent
  ];

  static moduleExports = [
    ...SharedModuleConstants.AngularModules,
    ...SharedModuleConstants.LayoutComponents,
    ...SharedModuleConstants.NgXBootstrapExportModules,
    NgSelectModule,
    ...SharedModuleConstants.AmcsPresentationWrappers,
    CurrencyPipe,
    ...SharedModuleConstants.AngularMatModules,
    ClipboardModule,
    CalendarModule,
    DropzoneModule,
    HeaderComponent,
    TranslateModule,
    AmcsNavbarComponent,
    AmcsContactComponent,
    AmcsFilterComponent,
    AmcsFilterInlineComponent,
    AmcsAppliedFilterListComponent,
    AmcsFilterMobileComponent,
    AmcsPictureViewerComponent,
    AmcsCarouselComponent,
    BreakWordPipe,
    KeyMaskPipe,
    MinusToParenthesis,
    AmcsLeafletMapComponent,
    InstrumentationDirective,
    InstrumentationListenDirective,
    AmcsDateRangeFilterComponent,
    LocalizedDatePipe,
    ImageManipulationComponent,
    FocusOnErrorDirective,
    FormSavingDirective,
    AmcsGridFormColumnDirective,
    AmcsDragDropComponent,
    ChartsModule,
    AmcsDataInputGridColumnDirective,
    ClickOutsideDirective,
    TakeFocusDirective,
    ImageViewerDirective,
    ZXingScannerModule,
    AmcsBarcodeScannerComponent,
    AmcsComponentFilterComponent,
    AmcsSignaturePadComponent,
    ScrollingModule,
    NumberFormatPipe,
    GlossaryPipe,
    CurrencyFormatPipe,
    AutomationLocatorDirective,
    AbsoluteValuePipe,
    TaxRateFormatPipe,
    IndexComponent,
    ObjectToDatePipe,
    AmcsBrowserGridEditorLayoutComponent,
    AmcsBrowserEditorTileComponent,
    PluraliseItemCountPipe,
    AmcsChangeLogComponent
  ];

  static moduleImports = [
    ...SharedModuleConstants.AngularModules,
    ...SharedModuleConstants.NgXBootstrapModules,
    ...SharedModuleConstants.AngularMatModules,
    ChartsModule,
    NgxCurrencyModule,
    ImageCropperModule,
    DropzoneModule,
    ClipboardModule,
    NgSelectModule,
    LeafletModule,
    LeafletDrawModule,
    LeafletMarkerClusterModule,
    ZXingScannerModule,
    ScrollingModule
  ];

  static moduleDeclarations = [
    AmcsFormLabelComponent,
    SharedModuleConstants.LayoutComponents,
    SharedModuleConstants.AmcsPresentationWrappers,
    AmcsNavbarComponent,
    AmcsCalendarDesktopCellComponent,
    CalendarOpenDayEventsComponent,
    HeaderDesktopComponent,
    HeaderMobileComponent,
    AmcsContactComponent,
    AmcsFilterComponent,
    AmcsCalendarWeekEventComponent,
    AmcsCalendarAllDayEventComponent,
    AmcsPictureViewerComponent,
    AmcsCarouselComponent,
    ImageViewerDirective,
    TakeFocusDirective,
    ClickOutsideDirective,
    InstrumentationDirective,
    InstrumentationListenDirective,
    AmcsGridFormColumnDirective,
    AmcsFilterInlineComponent,
    AmcsAppliedFilterListComponent,
    AmcsFilterMobileComponent,
    BreakWordPipe,
    KeyMaskPipe,
    MinusToParenthesis,
    AmcsLeafletMapComponent,
    AmcsMobileStepperComponent,
    AmcsDesktopStepperComponent,
    AmcsDateRangeFilterComponent,
    LocalizedDatePipe,
    ImageManipulationComponent,
    FocusOnErrorDirective,
    FormSavingDirective,
    AmcsSelectComponent,
    AmcsDragDropComponent,
    AmcsChartComponent,
    AmcsTimepickerDeprecatedComponent,
    AmcsDataInputGridColumnDirective,
    DefaultActionDetailsComponent,
    AmcsContainerTileComponent,
    AmcsSelectWithSearchComponent,
    HeaderMenuComponent,
    AmcsModalLookupSelectComponent,
    AmcsCostTemplateSelectorComponent,
    CostTemplateDetailsComponent,
    AmcsBarcodeScannerComponent,
    AmcsComponentFilterComponent,
    ComponentFilterEditorComponent,
    ModalDetailsComponent,
    AmcsSignaturePadComponent,
    NumberFormatPipe,
    GlossaryPipe,
    AutomationLocatorDirective,
    AbsoluteValuePipe,
    TaxRateFormatPipe,
    CurrencyFormatPipe,
    ErrorModalComponent,
    IndexComponent,
    AmcsMobileActionButtonsComponent,
    AmcsMobileSummaryComponent,
    ObjectToDatePipe,
    AmcsBrowserEditorTileComponent,
    AmcsBrowserGridEditorLayoutComponent,
    AmcsFormGroupComponent,
    PluraliseItemCountPipe,
    AmcsGridActionColumnHeaderComponent,
    AmcsChangeLogComponent
  ];

  static moduleProviders = [
    CurrencyPipe,
    LocalizedDatePipe,
    MinusToParenthesis,
    TaxRateFormatPipe,
    ObjectToDatePipe,
  ];
}
@NgModule({
  imports: [
    SharedModuleConstants.moduleImports,
    CalendarModule.forRoot({
      provide: DateAdapter,
      useFactory: adapterFactory
    }),
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient]
      },
      isolate: true
    })
  ],
  exports: [SharedModuleConstants.moduleExports],
  providers: [SharedModuleConstants.moduleProviders],
  declarations: [SharedModuleConstants.moduleDeclarations]
})
export class SharedModule {}
