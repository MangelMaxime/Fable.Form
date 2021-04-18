import { Union, Record } from "../../.fable/fable-library.3.1.11/Types.js";
import { union_type, record_type, bool_type, string_type } from "../../.fable/fable-library.3.1.11/Reflection.js";
import { View_ViewConfig$2, View_Validation, View_asHtml, succeed, group, append, checkboxField, radioField, meta, passwordField as passwordField_1, textField, View_Model$1, View_State, View_idle, View_Model$1$reflection } from "../../packages/Warded.Simple/Form.js";
import { Cmd_none } from "../../.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { printf, toConsole } from "../../.fable/fable-library.3.1.11/String.js";
import { FSharpResult$2 } from "../../.fable/fable-library.3.1.11/Choice.js";
import { Attributes } from "../../packages/Warded.Simple/TextField.js";
import { FieldConfig$4 } from "../../packages/Warded/Base.js";
import { curry, uncurry } from "../../.fable/fable-library.3.1.11/Util.js";
import { ofArray } from "../../.fable/fable-library.3.1.11/List.js";
import { Attributes as Attributes_1 } from "../../packages/Warded.Simple/RadioField.js";
import { Attributes as Attributes_2 } from "../../packages/Warded.Simple/CheckboxField.js";

export class FormResult extends Record {
    constructor(Email, Password, AcceptTerms, MakePublic) {
        super();
        this.Email = Email;
        this.Password = Password;
        this.AcceptTerms = AcceptTerms;
        this.MakePublic = MakePublic;
    }
}

export function FormResult$reflection() {
    return record_type("Page.SignUp.FormResult", [], FormResult, () => [["Email", string_type], ["Password", string_type], ["AcceptTerms", bool_type], ["MakePublic", string_type]]);
}

export class FormValues extends Record {
    constructor(Email, Password, RepeatPassword, AcceptTerms, MakePublic) {
        super();
        this.Email = Email;
        this.Password = Password;
        this.RepeatPassword = RepeatPassword;
        this.AcceptTerms = AcceptTerms;
        this.MakePublic = MakePublic;
    }
}

export function FormValues$reflection() {
    return record_type("Page.SignUp.FormValues", [], FormValues, () => [["Email", string_type], ["Password", string_type], ["RepeatPassword", string_type], ["AcceptTerms", bool_type], ["MakePublic", string_type]]);
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
    return union_type("Page.SignUp.Msg", [], Msg, () => [[["Item", View_Model$1$reflection(FormValues$reflection())]], [["Item", FormResult$reflection()]]]);
}

export function init() {
    return [View_idle(new FormValues("", "", "", false, "")), Cmd_none()];
}

export function update(msg, model) {
    if (msg.tag === 1) {
        const values = msg.fields[0];
        toConsole(printf("User login"));
        return [new View_Model$1(model.Values, new View_State(3, "You have been logged in successfully"), model.ErrorTracking), Cmd_none()];
    }
    else {
        const newModel = msg.fields[0];
        toConsole(printf("%A"))(newModel.Values);
        return [newModel, Cmd_none()];
    }
}

export const form = (() => {
    const emailField = textField(new FieldConfig$4((value) => ((value.indexOf("@") >= 0) ? (new FSharpResult$2(0, value)) : (new FSharpResult$2(1, "The e-mail address must contain a \u0027@\u0027 symbol"))), (values) => values.Email, (newValue, values_1) => (new FormValues(newValue, values_1.Password, values_1.RepeatPassword, values_1.AcceptTerms, values_1.MakePublic)), (_arg1) => (void 0), new Attributes("Email", "some@email.com")));
    const passwordField = passwordField_1(new FieldConfig$4((arg0) => (new FSharpResult$2(0, arg0)), (values_2) => values_2.Password, (newValue_1, values_3) => (new FormValues(values_3.Email, newValue_1, values_3.RepeatPassword, values_3.AcceptTerms, values_3.MakePublic)), (_arg2) => (void 0), new Attributes("Password", "Your password")));
    const repeatPasswordField = meta()(uncurry(2, (values_4) => passwordField_1(new FieldConfig$4((value_1) => ((value_1 === values_4.Password) ? (new FSharpResult$2(0, void 0)) : (new FSharpResult$2(1, "The passwords do not match"))), (values_5) => values_5.RepeatPassword, (newValue_2, values_) => (new FormValues(values_.Email, values_.Password, newValue_2, values_.AcceptTerms, values_.MakePublic)), (_arg3) => (void 0), new Attributes("Repeat password", "Your password again...")))));
    const makePublicField = radioField(new FieldConfig$4((arg0_1) => (new FSharpResult$2(0, arg0_1)), (values_6) => values_6.MakePublic, (newValue_3, values_7) => (new FormValues(values_7.Email, values_7.Password, values_7.RepeatPassword, values_7.AcceptTerms, newValue_3)), (_arg4) => (void 0), new Attributes_1("Make your profile public ?", ofArray([["option-yes", "Yes"], ["option-no", "No"]]))));
    const termsAndConditionField = checkboxField(new FieldConfig$4((arg0_2) => (new FSharpResult$2(0, arg0_2)), (values_8) => values_8.AcceptTerms, (newValue_4, values_9) => (new FormValues(values_9.Email, values_9.Password, values_9.RepeatPassword, newValue_4, values_9.MakePublic)), (_arg5) => (void 0), new Attributes_2("I agree with the terms and conditions")));
    const formOutput = (email, password, makePublic, acceptTerms) => (new Msg(1, new FormResult(email, password, acceptTerms, makePublic)));
    return append()(termsAndConditionField)(append()(makePublicField)(append()(group(append()(repeatPasswordField)(append()(passwordField)(succeed((password_1) => (() => password_1))))))(append()(emailField)(succeed(curry(4, formOutput))))));
})();

export function view(model, dispatch) {
    return View_asHtml(new View_ViewConfig$2(dispatch, (arg0) => (new Msg(0, arg0)), "Submit", "Loading", new View_Validation(1)))(form)(model);
}

export const code = "\nForm.succeed formOutput\n    |\u003e Form.append emailField\n    |\u003e Form.append\n        (\n            Form.succeed (fun password _ -\u003e password )\n            |\u003e Form.append passwordField\n            |\u003e Form.append repeatPasswordField\n            |\u003e Form.group\n        )\n    |\u003e Form.append makePublicField\n    |\u003e Form.append termsAndConditionField\n    ";

