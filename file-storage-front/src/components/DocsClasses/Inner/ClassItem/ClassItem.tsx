import React, {FC, memo} from "react";
import classesItems from "../DocsClassesItems.module.scss";
import {Controls} from "./Control";
import {Tags} from "./Tags/Tags";
import {ClassificationType} from "../../../../models/Classification";

type PropsControlType = {
    classification: ClassificationType
};

export const ClassItem: FC<PropsControlType> = memo(({classification}) => {

    return <tr className={classesItems.classesItem}>
        <td className={classesItems.classesItem__name}>{classification.name}</td>
        <td className={classesItems.classesItem__tags}><Tags tags={classification.classificationWords}/></td>
        <td className={classesItems.classesItem__controls}>
            <Controls/>
        </td>
    </tr>
});

