import React, {FC} from 'react';
import {InputText} from "../utils/Inputs/Text/InputText";
import {Button} from "../utils/Button/Button";
import classes from "./DocsClasses.module.scss";

type PropsType = {}

const DocsClasses: FC<PropsType> = (props) => {
    return (
        <div className={classes.block}>
            <h2 className={classes.h2}>Классификации документов</h2>
            <div className={classes.content}>
                <div className={classes.controls}>
                    <InputText className={classes.controlInput}/>
                    <Button className={classes.controlBtn}>Создать классификацию</Button>
                </div>
                <div className={classes.classes}>
                    <div>Техническое задание</div>
                    <div>Тагс</div>
                    <div>Три точки</div>
                </div>
            </div>
        </div>
    );
}


const Empty = () => {
    return <div className={classes.classesEmpty}>Классификации документов пока не созданы</div>;
}

export default DocsClasses;