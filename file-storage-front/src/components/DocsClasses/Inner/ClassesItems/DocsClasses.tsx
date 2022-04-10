import React, {FC} from 'react';
import classes from "./../../DocsClasses.module.scss";
import {ClassItem} from "../ClassItem/ClassItem";

type PropsType = {}

const ClassesItems: FC<PropsType> = (props) => {
    return (
        <table className={classes.classes}>
            <tbody>
            <ClassItem/>
            <ClassItem/>
            <ClassItem/>
            <ClassItem/>
            <ClassItem/>
            </tbody>
        </table>

    );
};


export default ClassesItems;
