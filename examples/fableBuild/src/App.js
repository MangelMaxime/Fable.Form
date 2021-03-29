import { Union, Record } from "../.fable/fable-library.3.1.11/Types.js";
import { union_type, record_type, string_type } from "../.fable/fable-library.3.1.11/Reflection.js";
import { ViewConfig$2, Validation, asHtml, Model$1, State as State_1, idle, Model$1$reflection } from "../packages/View.js";
import { Cmd_batch, Cmd_map, Cmd_none } from "../.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { succeed, append, textField } from "../packages/Form.js";
import { FSharpResult$2 } from "../.fable/fable-library.3.1.11/Choice.js";
import { Attributes } from "../packages/Base/TextField.js";
import { FieldConfig$4 } from "../packages/Base.js";
import { Program_Internal_withReactSynchronousUsing } from "../.fable/Fable.Elmish.React.3.0.1/react.fs.js";
import { lazyView2With } from "../.fable/Fable.Elmish.HMR.4.1.0/common.fs.js";
import { ProgramModule_map, ProgramModule_runWith, ProgramModule_mkProgram, ProgramModule_withConsoleTrace } from "../.fable/Fable.Elmish.3.1.0/program.fs.js";
import { Internal_saveState, Model$1 as Model$1_1, Msg$1, Internal_tryRestoreState } from "../.fable/Fable.Elmish.HMR.4.1.0/hmr.fs.js";
import { value as value_1 } from "../.fable/fable-library.3.1.11/Option.js";
import { ofArray, singleton } from "../.fable/fable-library.3.1.11/List.js";
import { uncurry } from "../.fable/fable-library.3.1.11/Util.js";

export class Values extends Record {
    constructor(Email, Password) {
        super();
        this.Email = Email;
        this.Password = Password;
    }
}

export function Values$reflection() {
    return record_type("Examples.Values", [], Values, () => [["Email", string_type], ["Password", string_type]]);
}

export class Msg extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["FormChanged", "LogIn"];
    }
}

export function Msg$reflection() {
    return union_type("Examples.Msg", [], Msg, () => [[["Item", Model$1$reflection(Values$reflection())]], [["Item1", string_type], ["Item2", string_type]]]);
}

export function init() {
    return [idle(new Values("", "")), Cmd_none()];
}

export function update(msg, model) {
    if (msg.tag === 1) {
        const password = msg.fields[1];
        const emailAddress = msg.fields[0];
        return [new Model$1(model.Values, new State_1(3, "You have been logged in successfully"), model.ErrorTracking), Cmd_none()];
    }
    else {
        const newModel = msg.fields[0];
        return [newModel, Cmd_none()];
    }
}

export const emailField = textField(new FieldConfig$4((arg0) => (new FSharpResult$2(0, arg0)), (values) => values.Email, (newValue, values_1) => (new Values(newValue, values_1.Password)), (_arg1) => (void 0), new Attributes("Email", "some@email.com")));

export const passwordField = textField(new FieldConfig$4((arg0) => (new FSharpResult$2(0, arg0)), (values) => values.Password, (newValue, values_1) => (new Values(values_1.Email, newValue)), (_arg1) => (void 0), new Attributes("Email", "some@email.com")));

export const form = append()(passwordField)(append()(emailField)(succeed((email) => ((password) => (new Msg(1, email, password))))));

export function view(model, dispatch) {
    return asHtml(new ViewConfig$2(dispatch, (arg0) => (new Msg(0, arg0)), "Submit", "Loading", new Validation(1)))(form)(model);
}

(function () {
    const program_3 = Program_Internal_withReactSynchronousUsing((equal, view_1, state, dispatch_1) => lazyView2With(equal, view_1, state, dispatch_1), "root", ProgramModule_withConsoleTrace(ProgramModule_mkProgram(init, (msg, model) => update(msg, model), (model_1, dispatch) => view(model_1, dispatch))));
    let hmrState = null;
    const hot = module.hot;
    if (!(hot == null)) {
        window.Elmish_HMR_Count = ((window.Elmish_HMR_Count == null) ? 0 : (window.Elmish_HMR_Count + 1));
        const value = hot.accept();
        const matchValue = Internal_tryRestoreState(hot);
        if (matchValue == null) {
        }
        else {
            const previousState = value_1(matchValue);
            hmrState = previousState;
        }
    }
    const map = (tupledArg) => {
        const model_2 = tupledArg[0];
        const cmd = tupledArg[1];
        return [model_2, Cmd_map((arg0) => (new Msg$1(0, arg0)), cmd)];
    };
    const mapUpdate = (update_1, msg_1, model_3) => {
        let msg_2, userModel, patternInput, newModel, cmd_2;
        const patternInput_1 = map((msg_1.tag === 1) ? [new Model$1_1(0), Cmd_none()] : (msg_2 = msg_1.fields[0], (model_3.tag === 1) ? (userModel = model_3.fields[0], (patternInput = update_1(msg_2, userModel), (newModel = patternInput[0], (cmd_2 = patternInput[1], [new Model$1_1(1, newModel), cmd_2])))) : [model_3, Cmd_none()]));
        const newModel_1 = patternInput_1[0];
        const cmd_3 = patternInput_1[1];
        hmrState = newModel_1;
        return [newModel_1, cmd_3];
    };
    const createModel = (tupledArg_1) => {
        const model_4 = tupledArg_1[0];
        const cmd_4 = tupledArg_1[1];
        return [new Model$1_1(1, model_4), cmd_4];
    };
    const mapInit = (init_1) => {
        if (hmrState == null) {
            return (arg_2) => createModel(map(init_1(arg_2)));
        }
        else {
            return (_arg1) => [hmrState, Cmd_none()];
        }
    };
    const mapSetState = (setState, model_5, dispatch_2) => {
        if (model_5.tag === 1) {
            const userModel_1 = model_5.fields[0];
            setState(userModel_1, (arg_3) => dispatch_2(new Msg$1(0, arg_3)));
        }
    };
    let hmrSubscription;
    const handler = (dispatch_3) => {
        if (!(hot == null)) {
            hot.dispose((data) => {
                Internal_saveState(data, hmrState);
                return dispatch_3(new Msg$1(1));
            });
        }
    };
    hmrSubscription = singleton(handler);
    const mapSubscribe = (subscribe, model_6) => {
        if (model_6.tag === 1) {
            const userModel_2 = model_6.fields[0];
            return Cmd_batch(ofArray([Cmd_map((arg0_2) => (new Msg$1(0, arg0_2)), subscribe(userModel_2)), hmrSubscription]));
        }
        else {
            return Cmd_none();
        }
    };
    const mapView = (view_2, model_7, dispatch_4) => {
        if (model_7.tag === 1) {
            const userModel_3 = model_7.fields[0];
            return view_2(userModel_3, (arg_4) => dispatch_4(new Msg$1(0, arg_4)));
        }
        else {
            throw (new Error("\nYour are using HMR and this Elmish application has been marked as inactive.\n\nYou should not see this message\n                    "));
        }
    };
    ProgramModule_runWith(void 0, ProgramModule_map(uncurry(2, mapInit), mapUpdate, mapView, mapSetState, mapSubscribe, program_3));
})();

