import { Union } from "../.fable/fable-library.3.1.11/Types.js";
import { union_type } from "../.fable/fable-library.3.1.11/Reflection.js";
import { oneOf, top, s, map } from "../.fable/Fable.Elmish.Browser.3.0.4/parser.fs.js";
import { ofArray } from "../.fable/fable-library.3.1.11/List.js";
import { Navigation_newUrl, Navigation_modifyUrl } from "../.fable/Fable.Elmish.Browser.3.0.4/navigation.fs.js";

export class Route extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Home", "SignUp", "Login", "NotFound"];
    }
}

export function Route$reflection() {
    return union_type("Router.Route", [], Route, () => [[], [], [], []]);
}

function toHash(page) {
    const segmentsPart = (page.tag === 1) ? "sign-up" : ((page.tag === 2) ? "login" : ((page.tag === 3) ? "not-found" : "home"));
    return "#" + segmentsPart;
}

export const routeParser = (() => {
    const parsers = ofArray([map(new Route(0), s("home")), map(new Route(1), s("sign-up")), map(new Route(2), s("login")), map(new Route(0), (state) => top(state))]);
    return (state_1) => oneOf(parsers, state_1);
})();

export function href(route) {
    return ["href", toHash(route)];
}

export function modifyUrl(route) {
    return Navigation_modifyUrl(toHash(route));
}

export function newUrl(route) {
    return Navigation_newUrl(toHash(route));
}

export function modifyLocation(route) {
    window.location.href = toHash(route);
}

