//@ts-check
import { rd, rdom, rd$, escape$, RDOMListHelper } from "../../../js/rdom.js";
import mdcrd from "../utils/mdcrd.js";
import { FrontendBasicPanel } from "./basic.js";
import { FrontendAccountsPanel } from "./accounts.js";

/**
 * @typedef {import("material-components-web")} mdc
 */
/** @type {import("material-components-web")} */
const mdc = window["mdc"]; // mdc

/**
 */

export class FrontendWhitelistPanel extends FrontendBasicPanel {
  /**
   * @param {import("../frontend.js").Frontend} frontend
   */
  constructor(frontend) {
    super(frontend);
    this.header = "Whitelist";
    this.ep = "/api/whitelist";
    /** @type {string[]} */
    this.data = [];
    this.input = null;
  }

  render(el) {
    return (this.el = rd$(el || this.el)`
    <div class="panel" ${rd.toggleClass("panelType", "panel-" + this.id)}=${true}>
      ${(el) => this.renderHeader(el)}
      ${mdcrd.progress(this.progress)}
      ${(el) => this.renderInput(el)}
      ${(el) => this.renderBody(el)}
    </div>`);
  }

  renderInput(el) {
    // Render input only once.
    if (this.elInput) return this.elInput;

    return (this.elInput = rd$(el || this.elInput)`
      <div class="panel-input">
      ${mdcrd.textField("", "", null, () => {
        this.refresh();
      })}
      ${mdcrd.iconButton("Clear", "clear", () => {
        this.input.value = "";
        this.refresh();
      })}
      </div>`);
  }

  async update() {
    this.data = await fetch(this.ep).then((r) => r.json());
    this.subheader = "(" + this.data.length + ")";
    this.rebuildList();
  }

  rebuildList() {
    // @ts-ignore
    this.input = this.elInput.getElementsByTagName("input")[0];
    let filter = this.input.value.trim().toLowerCase();

    console.log(FrontendAccountsPanel["instance"].data);

    this.list = this.data
      .map(
        (p) =>
          FrontendAccountsPanel["instance"].data.find((pl) => pl.UID == p) ?? {
            UID: p,
            Name: p,
          },
      )
      .filter(
        (p) => filter == "" || p.FullName.toLowerCase().indexOf(filter) >= 0,
      )
      .map((p) => (el) => {
        el = mdcrd.list.item(
          (el) => rd$(el)`
        <span>
        <b>${p.Name}</b> <i>(#${this.frontend.censor(p.UID)})</i><br>
        </span>`,
        )(el);

        this.frontend.dom.setContext(
          el,
          [
            "error_outline",
            `WhiteList Remove ${p.Name}`,
            () => this.frontend.dialog.wlrm(p.Name, p.UID),
          ],
          [
            "content_copy",
            `Copy Name: ${p.Name}`,
            () => navigator.clipboard.writeText(p.Name),
          ],
          [
            "content_copy",
            `Copy UID: ${p.UID}`,
            () => navigator.clipboard.writeText(p.UID),
          ],
        );

        return el;
      });
  }
}
