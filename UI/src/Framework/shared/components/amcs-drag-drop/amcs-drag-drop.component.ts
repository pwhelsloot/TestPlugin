import { Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, Renderer2, ViewChild } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { DropzoneComponent, DropzoneConfigInterface } from 'ngx-dropzone-wrapper';
import { fromEvent, Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, withLatestFrom } from 'rxjs/operators';
import { DragAndDropFromGroup } from './drag-and-drop-form-group.model';
import { DragAndDropItem } from './drag-and-drop-item.model';

/**
 * @todo Rename to amcs-file-dropzone
 */
@Component({
  selector: 'app-amcs-drag-drop',
  templateUrl: './amcs-drag-drop.component.html',
  styleUrls: ['./amcs-drag-drop.component.scss']
})
export class AmcsDragDropComponent extends AutomationLocatorDirective implements OnInit, OnDestroy {
  @Input() config: DropzoneConfigInterface;
  @Input() maxFiles: number;

  @Output() onMaxFilesAdded: EventEmitter<[boolean, number]> = new EventEmitter<[boolean, number]>();
  @Output() onMaxFileSizeExceeded: EventEmitter<[string, number, number]> = new EventEmitter<[string, number, number]>();
  @Output() onFilesChanged: EventEmitter<DragAndDropItem[]> = new EventEmitter<DragAndDropItem[]>();
  @Output() onFilesValidated: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output() onInvalidFileType: EventEmitter<any> = new EventEmitter<any>();
  @Output() onZeroFileSize: EventEmitter<boolean> = new EventEmitter<boolean>();

  @ViewChild('dropzoneComponent') dropzoneComponent: DropzoneComponent;

  constructor(protected elRef: ElementRef, protected renderer: Renderer2, private translationService: SharedTranslationsService) {
    super(elRef, renderer);
  }

  private onMaxFilesSubject: Subject<boolean> = new Subject<boolean>();
  private onMaxFileSizeSubject: Subject<[string, number, number]> = new Subject<[string, number, number]>();

  private eventListeners: DragAndDropFromGroup[] = [];

  private maxFileSubscription: Subscription;
  private maxFileSizeSubscription: Subscription;

  ngOnInit() {
    this.setupMaxFileSizeStream();
    this.setupMaxFilesStream();
  }

  ngOnDestroy() {
    // Remove any existing event listeners
    this.eventListeners
      .filter((listener) => isTruthy(listener.subscriptions))
      .forEach((listener) => {
        listener.subscriptions.forEach((subscription) => subscription.unsubscribe());
      });

    if (this.maxFileSizeSubscription) {
      this.maxFileSizeSubscription.unsubscribe();
    }

    if (this.maxFileSubscription) {
      this.maxFileSubscription.unsubscribe();
    }
  }

  clearFiles() {
    this.getDropzone().removeAllFiles();
  }

  disableDragAndDrop(message: string) {
    this.getDropzone().disable();
    this.onMaxFilesSubject.next(true);
    const messageContainer = this.dropzoneComponent.directiveRef['elementRef'].nativeElement
      .querySelector('.dz-message')
      .querySelector('.dz-text');
    messageContainer.innerHTML = message;
  }

  enableDragAndDrop() {
    this.getDropzone().enable();
    this.onMaxFilesSubject.next(false);
    const messageContainer = this.dropzoneComponent.directiveRef['elementRef'].nativeElement
      .querySelector('.dz-message')
      .querySelector('.dz-text');
    messageContainer.innerHTML = this.config.dictDefaultMessage;
  }

  onAddedFile(file: any) {
    if (isTruthy(file) && file.size === 0) {
      this.getDropzone().removeFile(file);
      this.onZeroFileSize.emit(true);
      return;
    }

    // Mark file as complete to remove progress bar
    this.getDropzone().emit('complete', file);

    // Does the picture have any input fields associated with it?
    const inputs = file.previewElement.querySelectorAll('input');

    // If so, cycle through each input and attach a listener to each so we can monitor when the fields have changed
    // and alert outside componets via our event emitter. This is also where we check to see if a input field is
    // marked as required, and if so, run the necessary checks and alert outside components on whether the form group
    // is valid
    if (isTruthy(inputs)) {
      const eventListenerItem: DragAndDropFromGroup = {
        uuid: file.upload.uuid,
        subscriptions: [],
        isFormGroupValid: []
      };
      this.eventListeners.push(eventListenerItem);

      inputs.forEach((input: any, index: number) => {
        if (input.hasAttribute('data-amcs-defaultvalue') && isTruthy(file[input.getAttribute('data-amcs-defaultvalue')])) {
          input.value = file[input.getAttribute('data-amcs-defaultvalue')];
        }

        const subscription = fromEvent(input, 'keyup')
          .pipe(debounceTime(100), distinctUntilChanged())
          .subscribe(() => {
            const idx = index;
            this.formatFiles(this.getDropzoneFiles());
            this.checkRequired(input, eventListenerItem.uuid, idx);
          });

        eventListenerItem.subscriptions.push(subscription);

        if (input.getAttribute('type') === 'checkbox') {
          this.addToSubscriptionCheckBoxClick(input, index, eventListenerItem);
        }


        this.checkRequired(input, eventListenerItem.uuid, index);
      });
    }

    // Does the picture have any textarea fields associated with it?
    const textareas = file.previewElement.querySelectorAll('textarea');

    // If so, cycle through each textarea and attach a listener to each so we can monitor when the fields have changed
    // and alert outside componets via our event emitter. This is also where we check to see if a textarea field is
    // marked as required, and if so, run the necessary checks and alert outside components on whether the form group
    // is valid
    if (isTruthy(textareas)) {
      const eventListenerItem: DragAndDropFromGroup = {
        uuid: file.upload.uuid,
        subscriptions: [],
        isFormGroupValid: []
      };
      this.eventListeners.push(eventListenerItem);

      textareas.forEach((textarea: any, index: number) => {
        if (textarea.hasAttribute('data-amcs-defaultvalue') && isTruthy(file[textarea.getAttribute('data-amcs-defaultvalue')])) {
          textarea.value = file[textarea.getAttribute('data-amcs-defaultvalue')];
        }

        const subscription = fromEvent(textarea, 'keyup')
          .pipe(debounceTime(100), distinctUntilChanged())
          .subscribe(() => {
            const idx = index;
            this.formatFiles(this.getDropzoneFiles());
            this.checkRequired(textarea, eventListenerItem.uuid, idx);
          });

        eventListenerItem.subscriptions.push(subscription);
        this.checkRequired(textarea, eventListenerItem.uuid, index);
      });
    }

    // Does the picture have any dropdown fields associated with it?
    const dropdowns = file.previewElement.querySelectorAll('select');
    if (isTruthy(dropdowns)) {
      this.addToSubscriptionSelectChange(file, dropdowns);
    }

    this.formatFiles(this.getDropzoneFiles());
  }

  onMaxFilesReached(file: any) {
    this.onMaxFilesSubject.next(true);
  }

  onMaxFilesExceeded(file: any) {
    this.getDropzone().removeFile(file);
  }

  onError(event: any) {
    const file = event[0];
    const error: string = event[1];

    if (error.startsWith('File is too big')) {
      this.onMaxFileSizeSubject.next([file.name, file.size / 1000000, this.config.maxFilesize]);
      this.getDropzone().removeFile(file);
    } else if (error.startsWith('You can\'t upload files of this type')) {
      this.onInvalidFileType.next();
      this.getDropzone().removeFile(file);
    }
  }

  onRemoveFile(file: any) {
    const files = this.getDropzoneFiles();

    if (files.length < this.config.maxFiles) {
      this.onMaxFilesSubject.next(false);
    }

    this.formatFiles(files);

    if (isTruthy(this.eventListeners.find((x) => x.uuid === file.upload.uuid))) {
      const eventListener = this.eventListeners.find((x) => x.uuid === file.upload.uuid);
      eventListener.subscriptions.forEach((subscription) => subscription.unsubscribe());
      this.eventListeners = this.eventListeners.filter((x) => x.uuid !== file.upload.uuid);
    }

    this.isComponentValid();
  }

  private addToSubscriptionSelectChange(file: any, dropdowns: any) {
    const eventListenerItem: DragAndDropFromGroup = {
      uuid: file.upload.uuid,
      subscriptions: [],
      isFormGroupValid: []
    };
    this.eventListeners.push(eventListenerItem);

    dropdowns.forEach((dropdown: any, index: number) => {
      if (dropdown.hasAttribute('data-amcs-defaultvalue') && isTruthy(file[dropdown.getAttribute('data-amcs-defaultvalue')])) {
        dropdown.value = file[dropdown.getAttribute('data-amcs-defaultvalue')];
      }

      const subscription = fromEvent(dropdown, 'change')
        .pipe(debounceTime(100), distinctUntilChanged())
        .subscribe(() => {
          const idx = index;
          this.formatFiles(this.getDropzoneFiles());
          this.checkRequired(dropdown, eventListenerItem.uuid, idx);
        });

      eventListenerItem.subscriptions.push(subscription);
      this.checkRequired(dropdown, eventListenerItem.uuid, index);
    });
  }

  private addToSubscriptionCheckBoxClick(input: any, index: number, eventListenerItem: DragAndDropFromGroup) {
    const subscriptionMouseClick = fromEvent(input, 'click')
      .pipe(debounceTime(100), distinctUntilChanged())
      .subscribe(() => {
        const idx = index;
        this.formatFiles(this.getDropzoneFiles());
        this.checkRequired(input, eventListenerItem.uuid, idx);
      });
    eventListenerItem.subscriptions.push(subscriptionMouseClick);
  }

  private getDropzone(): any {
    return this.dropzoneComponent.directiveRef['elementRef'].nativeElement.dropzone;
  }

  private getDropzoneFiles(): any {
    return this.getDropzone().files;
  }

  private checkRequired(input: any, uuid: string, index: number) {
    if (input.hasAttribute('data-amcs-isrequired') && !isTruthy(input.value)) {
      input.classList.add('has-error');
      this.eventListeners.find((x) => x.uuid === uuid).isFormGroupValid[index] = false;
    } else if (input.hasAttribute('data-amcs-isrequired')) {
      input.classList.remove('has-error');
      this.eventListeners.find((x) => x.uuid === uuid).isFormGroupValid[index] = true;
    }

    this.isComponentValid();
  }

  private isComponentValid() {
    this.onFilesValidated.emit(this.eventListeners.every((formGroup) => formGroup.isFormGroupValid.every((valid) => valid)));
  }

  private formatFiles(files: any[]) {
    const readFile = (file): Promise<string> => {
      const reader = new FileReader();

      return new Promise<string>((resolve, reject) => {
        reader.onload = (event) => {
          resolve((event.target['result'] as string).split(',')[1]);
        };

        reader.readAsDataURL(file);
      });
    };

    const results = files.map(async (file) => {
      const dataURL = await readFile(file);
      const data: DragAndDropItem = {
        dataURL,
        previewElement: file.previewElement,
        previewTemplate: file.previewTemplate,
        size: file.size,
        type: file.type,
        uuid: file.upload.uuid,
        name: file.name,
        comment: file.comment
      };

      return data;
    });

    Promise.all(results).then((images) => {
      this.onFilesChanged.emit(images);
    });
  }

  private setupMaxFilesStream() {
    this.maxFileSubscription = this.onMaxFilesSubject
      .pipe(withLatestFrom(this.translationService.translations), debounceTime(100))
      .subscribe((data) => {
        const isDisabled = data[0];
        const translations = data[1];

        const message = this.dropzoneComponent.directiveRef['elementRef'].nativeElement
          .querySelector('.dz-message')
          .querySelector('.dz-text');
        if (isDisabled) {
          const maxFiles = isTruthy(this.maxFiles) && this.maxFiles > 0 ? this.maxFiles : this.config.maxFiles;
          this.getDropzone().disable();
          message.innerHTML = (translations['dragAndDrop.maxFilesReached'] as string).replace('{{maxFiles}}', `${maxFiles}`);
        } else {
          this.getDropzone().enable();
          message.innerHTML = this.config.dictDefaultMessage;
        }

        this.onMaxFilesAdded.emit([isDisabled, this.config.maxFiles]);
      });
  }

  private setupMaxFileSizeStream() {
    this.maxFileSizeSubscription = this.onMaxFileSizeSubject.pipe(debounceTime(100)).subscribe((data) => {
      this.onMaxFileSizeExceeded.emit(data);
    });
  }
}
