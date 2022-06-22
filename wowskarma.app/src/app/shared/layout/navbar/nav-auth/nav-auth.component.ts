import { ChangeDetectionStrategy, Component } from "@angular/core";
import { NavigationEnd, Router } from "@angular/router";
import { shareReplay, startWith } from "rxjs";
import { filter, map } from "rxjs/operators";
import { AuthService } from "src/app/services/auth.service";

@Component({
  selector: "navbar-auth",
  templateUrl: "./nav-auth.component.html",
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NavAuthComponent {
  currentRelativePath$ = this.router.events.pipe(
    filter(e => e instanceof NavigationEnd),
    map(() => this.router.url),
    startWith(this.router.url),
    shareReplay(1)
  );

  constructor(public authService: AuthService, private router: Router) {

  }
}
