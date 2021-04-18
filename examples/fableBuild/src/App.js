import { Record, Union } from "../.fable/fable-library.3.1.11/Types.js";
import { code, view as view_3, update as update_2, init as init_3, Msg$reflection as Msg$reflection_1, FormValues$reflection } from "./Page/SignUp.js";
import { View_Model$1$reflection } from "../packages/Warded.Simple/Form.js";
import { code as code_1, view as view_4, update as update_3, init as init_2, Msg$reflection as Msg$reflection_2, FormValues$reflection as FormValues$reflection_1 } from "./Page/Login.js";
import { record_type, option_type, union_type } from "../.fable/fable-library.3.1.11/Reflection.js";
import { routeParser, Route, href, Route$reflection } from "./Router.js";
import { Cmd_batch, Cmd_none, Cmd_map } from "../.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { mapSecond, mapFirst } from "./Tuple.js";
import { createElement } from "react";
import { uncurry, createObj } from "../.fable/fable-library.3.1.11/Util.js";
import { singleton, ofArray } from "../.fable/fable-library.3.1.11/List.js";
import { Helpers_combineClasses } from "../.fable/Feliz.Bulma.2.11.0/ElementBuilders.fs.js";
import { Interop_reactApi } from "../.fable/Feliz.1.40.0/Interop.fs.js";
import { Program_Internal_withReactSynchronousUsing } from "../.fable/Fable.Elmish.React.3.0.1/react.fs.js";
import { lazyView2With } from "../.fable/Fable.Elmish.HMR.4.1.0/common.fs.js";
import { ProgramModule_Internal_toNavigableWith, ProgramModule_Internal_subscribe, ProgramModule_Internal_unsubscribe } from "../.fable/Fable.Elmish.Browser.3.0.4/navigation.fs.js";
import { parseHash } from "../.fable/Fable.Elmish.Browser.3.0.4/parser.fs.js";
import { ProgramModule_map, ProgramModule_runWith, ProgramModule_mkProgram, ProgramModule_withTrace } from "../.fable/Fable.Elmish.3.1.0/program.fs.js";
import { getMsgNameAndFields } from "./ConsoleTracer.js";
import { value as value_12, some } from "../.fable/fable-library.3.1.11/Option.js";
import { Internal_saveState, Model$1, Msg$1, Internal_tryRestoreState } from "../.fable/Fable.Elmish.HMR.4.1.0/hmr.fs.js";
import "../../style/style.scss";


export class Page extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Home", "SignUp", "Login", "NotFound"];
    }
}

export function Page$reflection() {
    return union_type("Examples.Page", [], Page, () => [[], [["Item", View_Model$1$reflection(FormValues$reflection())]], [["Item", View_Model$1$reflection(FormValues$reflection_1())]], []]);
}

export class Msg extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["SetRoute", "SignUpMsg", "LoginMsg"];
    }
}

export function Msg$reflection() {
    return union_type("Examples.Msg", [], Msg, () => [[["Item", option_type(Route$reflection())]], [["Item", Msg$reflection_1()]], [["Item", Msg$reflection_2()]]]);
}

export class Model extends Record {
    constructor(CurrentRoute, ActivePage) {
        super();
        this.CurrentRoute = CurrentRoute;
        this.ActivePage = ActivePage;
    }
}

export function Model$reflection() {
    return record_type("Examples.Model", [], Model, () => [["CurrentRoute", option_type(Route$reflection())], ["ActivePage", Page$reflection()]]);
}

function setRoute(optRoute, model) {
    const model_1 = new Model(optRoute, model.ActivePage);
    if (optRoute != null) {
        const route = optRoute;
        switch (route.tag) {
            case 2: {
                const patternInput_1 = init_2();
                const subModel_1 = patternInput_1[0];
                const subCmd_1 = patternInput_1[1];
                return [new Model(model_1.CurrentRoute, new Page(2, subModel_1)), Cmd_map((arg0_1) => (new Msg(2, arg0_1)), subCmd_1)];
            }
            case 0: {
                return [new Model(model_1.CurrentRoute, new Page(0)), Cmd_none()];
            }
            case 3: {
                return [new Model(model_1.CurrentRoute, new Page(3)), Cmd_none()];
            }
            default: {
                const patternInput = init_3();
                const subModel = patternInput[0];
                const subCmd = patternInput[1];
                return [new Model(model_1.CurrentRoute, new Page(1, subModel)), Cmd_map((arg0) => (new Msg(1, arg0)), subCmd)];
            }
        }
    }
    else {
        return [new Model(model_1.CurrentRoute, new Page(3)), Cmd_none()];
    }
}

function update(msg, model) {
    switch (msg.tag) {
        case 1: {
            const subMsg = msg.fields[0];
            const matchValue = model.ActivePage;
            if (matchValue.tag === 1) {
                const subModel = matchValue.fields[0];
                let tupledArg_2;
                let tupledArg_1;
                const tupledArg = update_2(subMsg, subModel);
                tupledArg_1 = mapFirst((arg0) => (new Page(1, arg0)), tupledArg[0], tupledArg[1]);
                tupledArg_2 = mapFirst((page) => (new Model(model.CurrentRoute, page)), tupledArg_1[0], tupledArg_1[1]);
                return mapSecond((cmd) => Cmd_map((arg0_1) => (new Msg(1, arg0_1)), cmd), tupledArg_2[0], tupledArg_2[1]);
            }
            else {
                return [model, Cmd_none()];
            }
        }
        case 2: {
            const subMsg_1 = msg.fields[0];
            const matchValue_1 = model.ActivePage;
            if (matchValue_1.tag === 2) {
                const subModel_1 = matchValue_1.fields[0];
                let tupledArg_5;
                let tupledArg_4;
                const tupledArg_3 = update_3(subMsg_1, subModel_1);
                tupledArg_4 = mapFirst((arg0_2) => (new Page(2, arg0_2)), tupledArg_3[0], tupledArg_3[1]);
                tupledArg_5 = mapFirst((page_1) => (new Model(model.CurrentRoute, page_1)), tupledArg_4[0], tupledArg_4[1]);
                return mapSecond((cmd_1) => Cmd_map((arg0_3) => (new Msg(2, arg0_3)), cmd_1), tupledArg_5[0], tupledArg_5[1]);
            }
            else {
                return [model, Cmd_none()];
            }
        }
        default: {
            const optRoute = msg.fields[0];
            return setRoute(optRoute, model);
        }
    }
}

export function init(location) {
    return setRoute(location, new Model(void 0, new Page(0)));
}

export function renderLink(route, linkText, description) {
    const children = ofArray([createElement("a", createObj(ofArray([href(route), ["children", linkText]]))), createElement("div", createObj(Helpers_combineClasses("content", singleton(["children", description]))))]);
    return createElement("li", {
        children: Interop_reactApi.Children.toArray(Array.from(children)),
    });
}

export function renderDemoPage(titleText, content, codeText) {
    let props_3;
    const children = ofArray([createElement("br", {}), (props_3 = ofArray([["className", "has-text-centered"], ["children", Interop_reactApi.Children.toArray([createElement("h5", createObj(Helpers_combineClasses("title is-5", ofArray([["className", "is-5"], ["children", titleText]]))))])]]), createElement("div", createObj(Helpers_combineClasses("content", props_3)))), createElement("hr", {}), createElement("pre", {
        className: "code-preview",
        children: Interop_reactApi.Children.toArray([createElement("code", {
            children: codeText.trim(),
        })]),
    }), createElement("hr", {}), content]);
    return createElement("div", {
        children: Interop_reactApi.Children.toArray(Array.from(children)),
    });
}

export function contentFromPage(page, dispatch) {
    let props_3, children;
    switch (page.tag) {
        case 1: {
            const subModel = page.fields[0];
            return renderDemoPage("Sign up", view_3(subModel, (arg) => {
                dispatch(new Msg(1, arg));
            }), code);
        }
        case 2: {
            const subModel_1 = page.fields[0];
            return renderDemoPage("Login", view_4(subModel_1, (arg_1) => {
                dispatch(new Msg(2, arg_1));
            }), code_1);
        }
        case 3: {
            return "Page not found";
        }
        default: {
            const elms = ofArray([createElement("br", {}), (props_3 = ofArray([["className", "has-text-centered"], ["children", Interop_reactApi.Children.toArray([createElement("h5", createObj(Helpers_combineClasses("title is-5", ofArray([["className", "is-5"], ["children", "List of examples"]]))))])]]), createElement("div", createObj(Helpers_combineClasses("content", props_3)))), createElement("hr", {}), (children = ofArray([renderLink(new Route(2), "Login", "A simple login form with 3 field"), renderLink(new Route(1), "Sign-up", "A form demonstrating how to handle external errors")]), createElement("ul", {
                children: Interop_reactApi.Children.toArray(Array.from(children)),
            }))]);
            return createElement("div", {
                className: "content",
                children: Interop_reactApi.Children.toArray(Array.from(elms)),
            });
        }
    }
}

export function view(model, dispatch) {
    let props;
    const elms = singleton((props = ofArray([["className", "is-8"], ["className", "is-offset-2"], ["children", contentFromPage(model.ActivePage, dispatch)]]), createElement("div", createObj(Helpers_combineClasses("column", props)))));
    return createElement("div", {
        className: "columns",
        children: Interop_reactApi.Children.toArray(Array.from(elms)),
    });
}

(function () {
    const program_5 = Program_Internal_withReactSynchronousUsing((equal, view_1, state, dispatch_2) => lazyView2With(equal, view_1, state, dispatch_2), "root", (() => {
        const onLocationChange = (dispatch_1) => {
            if (!(module.hot == null)) {
                if (module.hot.status() !== "idle") {
                    ProgramModule_Internal_unsubscribe();
                }
            }
            ProgramModule_Internal_subscribe(dispatch_1);
        };
        return ProgramModule_Internal_toNavigableWith((location_1) => parseHash(routeParser, location_1), (optRoute, model_2) => setRoute(optRoute, model_2), ProgramModule_withTrace((msg_1, _state) => {
            const patternInput = getMsgNameAndFields(Msg$reflection(), msg_1);
            const msg_3 = patternInput[0];
            const fields = patternInput[1];
            console.log(some(msg_3), fields);
        }, ProgramModule_mkProgram((location) => init(location), (msg, model) => update(msg, model), (model_1, dispatch) => view(model_1, dispatch))), onLocationChange);
    })());
    let hmrState = null;
    const hot = module.hot;
    if (!(hot == null)) {
        window.Elmish_HMR_Count = ((window.Elmish_HMR_Count == null) ? 0 : (window.Elmish_HMR_Count + 1));
        const value = hot.accept();
        const matchValue = Internal_tryRestoreState(hot);
        if (matchValue == null) {
        }
        else {
            const previousState = value_12(matchValue);
            hmrState = previousState;
        }
    }
    const map = (tupledArg) => {
        const model_3 = tupledArg[0];
        const cmd = tupledArg[1];
        return [model_3, Cmd_map((arg0) => (new Msg$1(0, arg0)), cmd)];
    };
    const mapUpdate = (update_1, msg_4, model_4) => {
        let msg_5, userModel, patternInput_1, newModel, cmd_2;
        const patternInput_2 = map((msg_4.tag === 1) ? [new Model$1(0), Cmd_none()] : (msg_5 = msg_4.fields[0], (model_4.tag === 1) ? (userModel = model_4.fields[0], (patternInput_1 = update_1(msg_5, userModel), (newModel = patternInput_1[0], (cmd_2 = patternInput_1[1], [new Model$1(1, newModel), cmd_2])))) : [model_4, Cmd_none()]));
        const newModel_1 = patternInput_2[0];
        const cmd_3 = patternInput_2[1];
        hmrState = newModel_1;
        return [newModel_1, cmd_3];
    };
    const createModel = (tupledArg_1) => {
        const model_5 = tupledArg_1[0];
        const cmd_4 = tupledArg_1[1];
        return [new Model$1(1, model_5), cmd_4];
    };
    const mapInit = (init_1) => {
        if (hmrState == null) {
            return (arg_2) => createModel(map(init_1(arg_2)));
        }
        else {
            return (_arg1) => [hmrState, Cmd_none()];
        }
    };
    const mapSetState = (setState, model_6, dispatch_3) => {
        if (model_6.tag === 1) {
            const userModel_1 = model_6.fields[0];
            setState(userModel_1, (arg_3) => dispatch_3(new Msg$1(0, arg_3)));
        }
    };
    let hmrSubscription;
    const handler = (dispatch_4) => {
        if (!(hot == null)) {
            hot.dispose((data) => {
                Internal_saveState(data, hmrState);
                return dispatch_4(new Msg$1(1));
            });
        }
    };
    hmrSubscription = singleton(handler);
    const mapSubscribe = (subscribe, model_7) => {
        if (model_7.tag === 1) {
            const userModel_2 = model_7.fields[0];
            return Cmd_batch(ofArray([Cmd_map((arg0_2) => (new Msg$1(0, arg0_2)), subscribe(userModel_2)), hmrSubscription]));
        }
        else {
            return Cmd_none();
        }
    };
    const mapView = (view_2, model_8, dispatch_5) => {
        if (model_8.tag === 1) {
            const userModel_3 = model_8.fields[0];
            return view_2(userModel_3, (arg_4) => dispatch_5(new Msg$1(0, arg_4)));
        }
        else {
            throw (new Error("\nYour are using HMR and this Elmish application has been marked as inactive.\n\nYou should not see this message\n                    "));
        }
    };
    ProgramModule_runWith(void 0, ProgramModule_map(uncurry(2, mapInit), mapUpdate, mapView, mapSetState, mapSubscribe, program_5));
})();

