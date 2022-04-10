import React, {FC, memo} from "react";
import classesItems from "../DocsClassesItems.module.scss";
import {Controls} from "./Control";
import {Tags} from "./Tags/Tags";

export const ClassItem: FC<PropsControlType> = memo(({}) => {

    return <tr className={classesItems.classesItem}>
        <td className={classesItems.classesItem__name}>Техническое задание</td>
        <td className={classesItems.classesItem__tags}><Tags/></td>
        <td className={classesItems.classesItem__controls}>
            <Controls/>
        </td>
    </tr>
});

type PropsControlType = {};
