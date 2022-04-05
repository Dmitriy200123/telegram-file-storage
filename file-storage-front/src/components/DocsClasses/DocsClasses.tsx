import React, {FC} from 'react';
import {InputText} from "../utils/Inputs/Text/InputText";
import {Button} from "../utils/Button/Button";
import classes from "./DocsClasses.module.scss";
import classesItems from "./DocsClassesItems.module.scss";
import Paginator from "../utils/Paginator/Paginator";
import {Controls} from "./Control";

type PropsType = {}

const DocsClasses: FC<PropsType> = (props) => {
    //
    return (
        <div className={classes.block}>
            <h2 className={classes.h2}>Классификации документов</h2>
            <div className={classes.background}>
                <div className={classes.content}>
                    <div className={classes.controls}>
                        <InputText className={classes.controlInput}/>
                        <Button className={classes.controlBtn}>Создать классификацию</Button>
                    </div>
                    <div className={classes.classes}>
                        <div className={classesItems.classesItem__name}>Техническое задание</div>
                        <div className={classesItems.classesItem__tags}>Тагс</div>
                        <div className={classesItems.classesItem__controls}>
                            {/*@ts-ignore*/}
                            <Controls/>
                        </div>
                    </div>
                </div>
                <Paginator paginator={{count: 10, currentPage: 1, filesInPage: 10}}/>
            </div>
        </div>
    );
};


const Empty = () => {
    return <div className={classes.classesEmpty}>Классификации документов пока не созданы</div>;
}


export default DocsClasses;
