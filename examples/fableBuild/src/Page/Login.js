import { Union, Record } from "../../.fable/fable-library.3.1.11/Types.js";
import { union_type, record_type, bool_type, string_type } from "../../.fable/fable-library.3.1.11/Reflection.js";
import { View_ViewConfig$2, View_Validation, View_asHtml, succeed, append, checkboxField, passwordField as passwordField_1, textField, View_Model$1, View_State, View_idle, View_Model$1$reflection } from "../../packages/Warded.Simple/Form.js";
import { create, tryParse, toString, T$reflection } from "../Types/EmailAddress.js";
import { Cmd_none } from "../../.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { FSharpResult$2, Result_Map } from "../../.fable/fable-library.3.1.11/Choice.js";
import { Attributes } from "../../packages/Warded.Simple/TextField.js";
import { FieldConfig$4 } from "../../packages/Warded/Base.js";
import { Attributes as Attributes_1 } from "../../packages/Warded.Simple/CheckboxField.js";
import { curry } from "../../.fable/fable-library.3.1.11/Util.js";
import { generateGithubUrl } from "../Global.js";

export class FormValues extends Record {
    constructor(Email, Password, RememberMe) {
        super();
        this.Email = Email;
        this.Password = Password;
        this.RememberMe = RememberMe;
    }
}

export function FormValues$reflection() {
    return record_type("Page.Login.FormValues", [], FormValues, () => [["Email", string_type], ["Password", string_type], ["RememberMe", bool_type]]);
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
    return union_type("Page.Login.Msg", [], Msg, () => [[["Item", View_Model$1$reflection(FormValues$reflection())]], [["Item1", T$reflection()], ["Item2", string_type], ["Item3", bool_type]]]);
}

export function init() {
    return [View_idle(new FormValues("", "", false)), Cmd_none()];
}

export function update(msg, model) {
    if (msg.tag === 1) {
        const rememberMe = msg.fields[2];
        const password = msg.fields[1];
        const email = msg.fields[0];
        return [new View_Model$1(model.Values, new View_State(3, "You have been logged in successfully"), model.ErrorTracking), Cmd_none()];
    }
    else {
        const newModel = msg.fields[0];
        return [newModel, Cmd_none()];
    }
}

export const form = (() => {
    const emailField = textField(new FieldConfig$4((arg) => Result_Map((arg00$0040) => toString(arg00$0040), tryParse(arg)), (values) => values.Email, (newValue, values_1) => (new FormValues(newValue, values_1.Password, values_1.RememberMe)), (_arg1) => (void 0), new Attributes("Email", "some@email.com")));
    const passwordField = passwordField_1(new FieldConfig$4((arg0) => (new FSharpResult$2(0, arg0)), (values_2) => values_2.Password, (newValue_1, values_3) => (new FormValues(values_3.Email, newValue_1, values_3.RememberMe)), (_arg2) => (void 0), new Attributes("Password", "Your password")));
    const rememberMe = checkboxField(new FieldConfig$4((arg0_1) => (new FSharpResult$2(0, arg0_1)), (values_4) => values_4.RememberMe, (newValue_2, values_5) => (new FormValues(values_5.Email, values_5.Password, newValue_2)), (_arg3) => (void 0), new Attributes_1("Remember me")));
    const formOutput = (email, password, rememberMe_1) => (new Msg(1, create(email), password, rememberMe_1));
    return append()(rememberMe)(append()(passwordField)(append()(emailField)(succeed(curry(3, formOutput)))));
})();

export function view(model, dispatch) {
    return View_asHtml(new View_ViewConfig$2(dispatch, (arg0) => (new Msg(0, arg0)), "Submit", "Loading", new View_Validation(1)))(form)(model);
}

export const code = "\nForm.succeed formOutput\n    |\u003e Form.append emailField\n    |\u003e Form.append passwordField\n    |\u003e Form.append rememberMe\n    ";

export const githubLink = generateGithubUrl("C:\\Users\\Maxime\\Documents\\Workspace\\Github\\MangelMaxime\\Warded\\examples\\src\\Page", "Login.fs");

