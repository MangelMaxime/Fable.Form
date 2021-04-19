import { Union, Record } from "../../.fable/fable-library.3.1.11/Types.js";
import { union_type, bool_type, option_type, record_type, string_type } from "../../.fable/fable-library.3.1.11/Reflection.js";
import { create, tryParse, toString, T$reflection } from "../Types/EmailAddress.js";
import { Name_create, Password_create, Password_tryParse, Password_toString, Name_tryParse, Name_toString, signUp as signUp_1, T$reflection as T$reflection_1, Name_T$reflection, Password_T$reflection } from "../Types/User.js";
import { View_ViewConfig$2, View_Validation, View_asHtml, succeed, group, append, radioField, meta, passwordField as passwordField_1, textField, View_Model$1, View_State, View_setLoading, View_idle, View_Model$1$reflection } from "../../packages/Warded.Simple/Form.js";
import { Result_Map, FSharpResult$2 } from "../../.fable/fable-library.3.1.11/Choice.js";
import { Cmd_OfPromise_perform, Cmd_none } from "../../.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { Attributes } from "../../packages/Warded.Simple/TextField.js";
import { FieldConfig$4 } from "../../packages/Warded/Base.js";
import { curry, uncurry } from "../../.fable/fable-library.3.1.11/Util.js";
import { ofArray } from "../../.fable/fable-library.3.1.11/List.js";
import { Attributes as Attributes_1 } from "../../packages/Warded.Simple/RadioField.js";
import { generateGithubUrl } from "../Global.js";

export class FieldError extends Record {
    constructor(Value, Error$) {
        super();
        this.Value = Value;
        this.Error = Error$;
    }
}

export function FieldError$reflection() {
    return record_type("Page.SignUp.FieldError", [], FieldError, () => [["Value", string_type], ["Error", string_type]]);
}

export function FieldErrorModule_create(value, error) {
    return new FieldError(value, error);
}

export class FormErrors extends Record {
    constructor(Email) {
        super();
        this.Email = Email;
    }
}

export function FormErrors$reflection() {
    return record_type("Page.SignUp.FormErrors", [], FormErrors, () => [["Email", option_type(FieldError$reflection())]]);
}

export class FormResult extends Record {
    constructor(Email, Password, Name, MakePublic) {
        super();
        this.Email = Email;
        this.Password = Password;
        this.Name = Name;
        this.MakePublic = MakePublic;
    }
}

export function FormResult$reflection() {
    return record_type("Page.SignUp.FormResult", [], FormResult, () => [["Email", T$reflection()], ["Password", Password_T$reflection()], ["Name", Name_T$reflection()], ["MakePublic", bool_type]]);
}

export class FormValues extends Record {
    constructor(Email, Password, RepeatPassword, Name, MakePublic, Errors) {
        super();
        this.Email = Email;
        this.Password = Password;
        this.RepeatPassword = RepeatPassword;
        this.Name = Name;
        this.MakePublic = MakePublic;
        this.Errors = Errors;
    }
}

export function FormValues$reflection() {
    return record_type("Page.SignUp.FormValues", [], FormValues, () => [["Email", string_type], ["Password", string_type], ["RepeatPassword", string_type], ["Name", string_type], ["MakePublic", string_type], ["Errors", FormErrors$reflection()]]);
}

export class Model extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["FillingForm", "SignedUp"];
    }
}

export function Model$reflection() {
    return union_type("Page.SignUp.Model", [], Model, () => [[["Item", View_Model$1$reflection(FormValues$reflection())]], [["Item", T$reflection_1()]]]);
}

export class Msg extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["FormChanged", "SignUp", "SignupAttempted"];
    }
}

export function Msg$reflection() {
    return union_type("Page.SignUp.Msg", [], Msg, () => [[["Item", View_Model$1$reflection(FormValues$reflection())]], [["Item", FormResult$reflection()]], [["Item", union_type("Microsoft.FSharp.Core.FSharpResult`2", [T$reflection_1(), string_type], FSharpResult$2, () => [[["ResultValue", T$reflection_1()]], [["ErrorValue", string_type]]])]]]);
}

export function init() {
    return [new Model(0, View_idle(new FormValues("", "", "", "", "", new FormErrors(void 0)))), Cmd_none()];
}

export function update(msg, model) {
    if (msg.tag === 1) {
        const formResult = msg.fields[0];
        if (model.tag === 0) {
            const formModel_1 = model.fields[0];
            const signUp = () => signUp_1(formResult.Email, formResult.Name, formResult.Password, formResult.MakePublic);
            return [new Model(0, View_setLoading(formModel_1)), Cmd_OfPromise_perform(signUp, void 0, (arg0_1) => (new Msg(2, arg0_1)))];
        }
        else {
            return [model, Cmd_none()];
        }
    }
    else if (msg.tag === 2) {
        if (msg.fields[0].tag === 1) {
            const error = msg.fields[0].fields[0];
            if (model.tag === 0) {
                const formModel_3 = model.fields[0];
                const values = formModel_3.Values;
                const errors = values.Errors;
                return [new Model(0, new View_Model$1(new FormValues(values.Email, values.Password, values.RepeatPassword, values.Name, values.MakePublic, new FormErrors(FieldErrorModule_create(values.Email, error))), new View_State(0), formModel_3.ErrorTracking)), Cmd_none()];
            }
            else {
                return [model, Cmd_none()];
            }
        }
        else {
            const user = msg.fields[0].fields[0];
            return [new Model(1, user), Cmd_none()];
        }
    }
    else {
        const formModel = msg.fields[0];
        if (model.tag === 0) {
            return [new Model(0, formModel), Cmd_none()];
        }
        else {
            return [model, Cmd_none()];
        }
    }
}

export function convertMakePublicOptionToBool(makePublic) {
    switch (makePublic) {
        case "option-yes": {
            return true;
        }
        default: {
            return false;
        }
    }
}

export const form = (() => {
    const emailField = textField(new FieldConfig$4((arg) => Result_Map((arg00$0040) => toString(arg00$0040), tryParse(arg)), (values) => values.Email, (newValue, values_1) => (new FormValues(newValue, values_1.Password, values_1.RepeatPassword, values_1.Name, values_1.MakePublic, values_1.Errors)), (_arg1) => {
        const errors = _arg1.Errors;
        const email = _arg1.Email;
        const matchValue = errors.Email;
        if (matchValue == null) {
            return void 0;
        }
        else {
            const emailError = matchValue;
            return (email === emailError.Value) ? emailError.Error : (void 0);
        }
    }, new Attributes("Email", "some@email.com")));
    const nameField = textField(new FieldConfig$4((arg_1) => Result_Map((arg00$0040_1) => Name_toString(arg00$0040_1), Name_tryParse(arg_1)), (values_2) => values_2.Name, (newValue_1, values_3) => (new FormValues(values_3.Email, values_3.Password, values_3.RepeatPassword, newValue_1, values_3.MakePublic, values_3.Errors)), (_arg2) => (void 0), new Attributes("Name", "Your name")));
    const passwordField = passwordField_1(new FieldConfig$4((arg_2) => Result_Map((arg00$0040_2) => Password_toString(arg00$0040_2), Password_tryParse(arg_2)), (values_4) => values_4.Password, (newValue_2, values_5) => (new FormValues(values_5.Email, newValue_2, values_5.RepeatPassword, values_5.Name, values_5.MakePublic, values_5.Errors)), (_arg3) => (void 0), new Attributes("Password", "Your password")));
    const repeatPasswordField = meta()(uncurry(2, (values_6) => passwordField_1(new FieldConfig$4((value) => ((value === values_6.Password) ? (new FSharpResult$2(0, void 0)) : (new FSharpResult$2(1, "The passwords do not match"))), (values_7) => values_7.RepeatPassword, (newValue_3, values_) => (new FormValues(values_.Email, values_.Password, newValue_3, values_.Name, values_.MakePublic, values_.Errors)), (_arg4) => (void 0), new Attributes("Repeat password", "Your password again...")))));
    const makePublicField = radioField(new FieldConfig$4((arg0) => (new FSharpResult$2(0, arg0)), (values_8) => values_8.MakePublic, (newValue_4, values_9) => (new FormValues(values_9.Email, values_9.Password, values_9.RepeatPassword, values_9.Name, newValue_4, values_9.Errors)), (_arg5) => (void 0), new Attributes_1("Make your profile public ?", ofArray([["option-yes", "Yes"], ["option-no", "No"]]))));
    const formOutput = (email_1, name, password, makePublic) => (new Msg(1, new FormResult(create(email_1), Password_create(password), Name_create(name), convertMakePublicOptionToBool(makePublic))));
    return append()(makePublicField)(append()(group(append()(repeatPasswordField)(append()(passwordField)(succeed((password_1) => (() => password_1))))))(append()(nameField)(append()(emailField)(succeed(curry(4, formOutput))))));
})();

export function view(model, dispatch) {
    if (model.tag === 1) {
        const user = model.fields[0];
        return "User signed up";
    }
    else {
        const formValues = model.fields[0];
        return View_asHtml(new View_ViewConfig$2(dispatch, (arg0) => (new Msg(0, arg0)), "Submit", "Loading", new View_Validation(1)))(form)(formValues);
    }
}

export const code = "\nForm.succeed formOutput\n    |\u003e Form.append emailField\n    |\u003e Form.append nameField\n    |\u003e Form.append\n        (\n            Form.succeed (fun password _ -\u003e password )\n            |\u003e Form.append passwordField\n            |\u003e Form.append repeatPasswordField\n            |\u003e Form.group\n        )\n    |\u003e Form.append makePublicField\n    ";

export const githubLink = generateGithubUrl("C:\\Users\\Maxime\\Documents\\Workspace\\Github\\MangelMaxime\\Warded\\examples\\src\\Page", "SignUp.fs");

