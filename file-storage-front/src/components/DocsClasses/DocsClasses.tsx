import React, {ChangeEvent, ChangeEventHandler, FC, useEffect, useState} from 'react';
import {InputText} from "../utils/Inputs/Text/InputText";
import {Button} from "../utils/Button/Button";
import classes from "./DocsClasses.module.scss";
import PaginatorNeNorm from "../utils/Paginator/PaginatorNeNorm";
import ClassesItems from "./Inner/ClassesItems/DocsClasses";
import DocsClassesModal from "./Modals/DocsClassesModal/DocsClassesModal";
import {useAppDispatch, useAppSelector} from "../../utils/hooks/reduxHooks";
import {classesDocsSlice} from "../../redux/classesDocs/classesDocsSlice";
import {Route, Switch} from "react-router-dom";
import OpenClassItem from "./OpenClassItem/OpenClassItem";
import {fetchClassifications, fetchCountClassifications} from "../../redux/classesDocs/classesDocsThunks";
import Paginator from "../utils/Paginator/Paginator";

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
    const classifications = useAppSelector(state => state.classesDocs.classifications);
    const count = useAppSelector(state => state.classesDocs.count);
    const [filters, setFilters] = useState({page: 1, take: 3, query: undefined as undefined | string});

    function fetchClasses() {
        dispatch(fetchCountClassifications(filters.query));
        dispatch(fetchClassifications({skip: filters.take * (filters.page - 1), take: filters.take, query: filters.query}));
    }

    useEffect(() => {
        fetchClasses();
    }, [filters]);

    useEffect(() => {
        if (classifications === null || classifications.length !== 0) {
            return fetchClasses();
        }

        if (filters.page > 1)
            setFilters((prev) => ({...prev, page: prev.page - 1}))
    }, [classifications?.length])

    function onChangeInput(e: ChangeEvent<HTMLInputElement>) {
        setFilters((prev) => ({...prev, page: 1, query: e.target.value}));
    }

    function onChangePage(page: number) {
        setFilters((prev) => ({...prev, page: page}));
    }

    return (
        <div className={classes.block}>
            <h2 className={classes.h2}>Классификации документов</h2>
            <div className={classes.background}>
                <div className={classes.content}>
                    <div className={classes.controls}>
                        <InputText className={classes.controlInput} onChange={onChangeInput}/>
                        <Button onClick={() => dispatch(openModal({type: "create"}))} className={classes.controlBtn}>Создать
                            классификацию</Button>
                    </div>
                    <ClassesItems classifications={classifications}/>
                </div>
                <Paginator pageHandler={onChangePage} current={filters.page} count={Math.ceil(count / filters.take)}/>
            </div>

        </div>
    );
};

const Empty = () => {
    return <div className={classes.classesEmpty}>Классификации документов пока не созданы</div>;
}


export default DocsClassesPage;
