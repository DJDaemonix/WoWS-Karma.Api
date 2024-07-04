import { ChangeDetectionStrategy, Component } from "@angular/core";
import { BehaviorSubject } from "rxjs";
import { NotificationsMenuComponent } from "../../../notifications/notifications-menu/notifications-menu.component";
import { AsyncPipe, NgIf } from "@angular/common";

@Component({
  standalone: true,
  selector: "app-notifications-button",
  templateUrl: "./notifications-button.component.html",
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    NotificationsMenuComponent,
    AsyncPipe,
    NgIf
  ]
})
export class NotificationsButtonComponent {
    count$ = new BehaviorSubject(0);

    constructor() { }
}
