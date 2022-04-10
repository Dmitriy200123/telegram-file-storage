import React, {FC} from 'react';
import {InputText} from "../utils/Inputs/Text/InputText";
import {Button} from "../utils/Button/Button";
import classes from "./DocsClasses.module.scss";
import Paginator from "../utils/Paginator/Paginator";
import ClassesItems from "./Inner/ClassesItems/DocsClasses";
import DocsClassesModal from "./Modals/DocsClassesModal/DocsClassesModal";
import {useAppDispatch, useAppSelector} from "../../utils/hooks/reduxHooks";
import {classesDocsSlice} from "../../redux/classesDocs/classesDocsSlice";
import {Route, Switch} from "react-router-dom";
import OpenClassItem from "./OpenClassItem/OpenClassItem";

type PropsType = {}

const {openModal, closeModal} = classesDocsSlice.actions
const DocsClassesPage: FC<PropsType> = (props) => {

    const {type, isOpen, args} = useAppSelector((state) => state.classesDocs.modal);
    const dispatch = useAppDispatch();
    return (<>
            <Switch>
                <Route path={"/docs-сlasses"} component={DocsClasses} exact={true}/>
                <Route path={"/docs-сlasses/:id"} component={OpenClassItem}/>
            </Switch>
            {isOpen &&
            <DocsClassesModal onOutsideClick={() => dispatch(closeModal())} modalType={type || "create"} args={args}/>}
        </>
    );
};

const DocsClasses: FC<PropsType> = (props) => {
    const dispatch = useAppDispatch();
    
    return (
        <div className={classes.block}>
            <h2 className={classes.h2}>Классификации документов</h2>
            <div className={classes.background}>
                <div className={classes.content}>
                    <div className={classes.controls}>
                        <InputText className={classes.controlInput}/>
                        <Button onClick={() => dispatch(openModal({type: "create"}))} className={classes.controlBtn}>Создать
                            классификацию</Button>
                    </div>
                    <ClassesItems/>
                </div>
                <Paginator paginator={{count: 10, currentPage: 1, filesInPage: 10}}/>
            </div>

        </div>
    );
};


const Empty = () => {
    return <div className={classes.classesEmpty}>Классификации документов пока не созданы</div>;
}


export default DocsClassesPage;
