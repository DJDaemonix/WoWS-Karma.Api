import { ChangeDetectionStrategy, Component, computed, inject, input } from "@angular/core";
import { NgbModal, NgbTooltip } from "@ng-bootstrap/ng-bootstrap";
import { PostModEditorComponent } from 'src/app/shared/modals/post-mod-edit/post-mod-editor.component';
import { PlayerPostDto } from "../../services/api/models/player-post-dto";
import { AuthService } from "../../services/auth.service";
import { PostDeleteComponent } from "../modals/post-delete/post-delete.component";
import { PostEditorComponent } from "../modals/post-editor/post-editor.component";
import { PostModDeleteComponent } from "../modals/post-mod-delete/post-mod-delete.component";
import { PostBorderColorPipe } from "../../services/pipes/post-border-color.pipe";
import { PlayerNamelinkComponent } from "../components/player-namelink/player-namelink.component";
import { FlairMarkupsComponent } from "./flair-markup/flair-markups.component";
import { MarkdownComponent } from "ngx-markdown";
import { CommonModule } from "@angular/common";
import { RouterLink } from "@angular/router";

@Component({
  selector: "app-post",
  templateUrl: "./post.component.html",
  imports: [
    PostBorderColorPipe,
    PlayerNamelinkComponent,
    FlairMarkupsComponent,
    MarkdownComponent,
    CommonModule,
    RouterLink,
    NgbTooltip
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})

export class PostComponent {
  public authService: AuthService = inject(AuthService);
  public post = input<PlayerPostDto>();
  public postDisplayType = input.required<"neutral" | "received" | "sent">();
  public isOwnerOrPrivileged = computed(() =>
    this.authService.userInfo$.value?.id === this.post()?.author?.id
    || this.authService.userInfo$.value?.roles?.includes("mod")
    || this.authService.userInfo$.value?.roles?.includes("admin")
    || this.authService.userInfo$.value?.roles?.includes("wg")
  );

  constructor(private modalService: NgbModal) {
  }

  get canDelete() {
    return !this.post()?.modLocked
      && this.authService.userInfo$.value?.id === this.post()?.author?.id;
  }

  get canEdit() {
    return this.canDelete && !this.post()?.readOnly;
  }

  openEditor() {
    return PostEditorComponent.OpenEditor(this.modalService, this.post()!);
  }

  openModEditor() {
    return PostModEditorComponent.OpenModal(this.modalService, this.post()!);
  }

  openDeleteModal() {
    return PostDeleteComponent.OpenModal(this.modalService, this.post()!);
  }

  openModDeleteModal() {
    return PostModDeleteComponent.OpenModal(this.modalService, this.post()!);
  }
}
