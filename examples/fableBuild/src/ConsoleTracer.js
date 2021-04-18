import { getUnionCaseFields, isUnion, getCaseFields, name, getUnionCases, getCaseName as getCaseName_1 } from "../.fable/fable-library.3.1.11/Reflection.js";
import { empty, cons } from "../.fable/fable-library.3.1.11/List.js";
import { join } from "../.fable/fable-library.3.1.11/String.js";
import { createObj } from "../.fable/fable-library.3.1.11/Util.js";
import { zip, map } from "../.fable/fable-library.3.1.11/Array.js";

export function getMsgNameAndFields(t, x) {
    const getCaseName = (t_1_mut, acc_mut, x_1_mut) => {
        getCaseName:
        while (true) {
            const t_1 = t_1_mut, acc = acc_mut, x_1 = x_1_mut;
            const caseName = getCaseName_1(x_1);
            const uci_1 = getUnionCases(t_1).find((uci) => (name(uci) === caseName));
            const acc_1 = cons(getCaseName_1(x_1), acc);
            const fields = getCaseFields(x_1);
            if ((fields.length === 1) ? isUnion(fields[0]) : false) {
                t_1_mut = getUnionCaseFields(uci_1)[0][1];
                acc_mut = acc_1;
                x_1_mut = fields[0];
                continue getCaseName;
            }
            else {
                const msgName = join("/", acc_1);
                const fields_2 = createObj(map((tupledArg) => {
                    const fi = tupledArg[0];
                    const v = tupledArg[1];
                    return [name(fi), v];
                }, zip(getUnionCaseFields(uci_1), fields)));
                return [msgName, fields_2];
            }
            break;
        }
    };
    if (isUnion(x)) {
        return getCaseName(t, empty(), x);
    }
    else {
        return ["Msg", x];
    }
}

