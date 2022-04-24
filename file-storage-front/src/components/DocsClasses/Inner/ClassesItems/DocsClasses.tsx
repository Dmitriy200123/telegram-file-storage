import React, {FC, memo} from 'react';
import classes from "./../../DocsClasses.module.scss";
import {ClassItem} from "../ClassItem/ClassItem";
import {ClassificationType} from "../../../../models/Classification";

type PropsType = {
    classifications: ClassificationType[] | undefined | null
}

const ClassesItems: FC<PropsType> = memo(({classifications}) => {
    return (
        <table className={classes.classes}>
            <tbody>
            {classifications?.map((c) => {
                return <ClassItem classification={c}/>
            })}
            </tbody>
        </table>

    );
});


export default ClassesItems;
