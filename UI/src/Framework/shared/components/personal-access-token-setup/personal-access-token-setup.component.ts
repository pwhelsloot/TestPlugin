import { Component, Output, EventEmitter, OnInit } from '@angular/core';
import { PersonalAccessTokenFormService } from '@core-module/services/persona-access-token/personal-access-token-form.service';

@Component({
  selector: 'app-personal-access-token-setup',
  templateUrl: './personal-access-token-setup.component.html',
  styleUrls: ['./personal-access-token-setup.component.scss'],
  providers: [PersonalAccessTokenFormService]
})
export class PersonalAccessTokenSetupComponent implements OnInit {

  @Output() onReturn = new EventEmitter<void>();

  constructor(public formService: PersonalAccessTokenFormService) { }

  ngOnInit() {
    this.formService.buildForm();
    this.formService.onReturn = this.onReturn;
  }
}
