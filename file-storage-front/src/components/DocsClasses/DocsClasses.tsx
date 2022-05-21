import React, {ChangeEvent, FC, useEffect, useRef, useState} from 'react';
import {InputText} from "../utils/Inputs/Text/InputText";
import {Button} from "../utils/Button/Button";
import classes from "./DocsClasses.module.scss";
import ClassesItems from "./Inner/ClassesItems/DocsClasses";
import DocsClassesModal from "./Modals/DocsClassesModal/DocsClassesModal";
import {useAppDispatch, useAppSelector} from "../../utils/hooks/reduxHooks";
import {classesDocsSlice} from "../../redux/classesDocs/classesDocsSlice";
import {Route, Switch} from "react-router-dom";
import OpenClassItem from "./OpenClassItem/OpenClassItem";
import {fetchClassifications, fetchCountClassifications} from "../../redux/classesDocs/classesDocsThunks";
import Paginator from "../utils/Paginator/Paginator";
import {ReactComponent as PlusIcon} from "./../../assets/plus.svg";
import {ReactComponent as Search} from "../../assets/search.svg";

type PropsType = {}

const {openModal, closeModal, setIsFetchClassifications} = classesDocsSlice.actions
const DocsClassesPage: FC<PropsType> = () => {
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

const DocsClasses: FC<PropsType> = () => {
    const dispatch = useAppDispatch();
    const classifications = useAppSelector(state => state.classesDocs.classifications);
    const count = useAppSelector(state => state.classesDocs.count);
    const isFetchClassifications = useAppSelector(state => state.classesDocs.fetchClassifications);
    const [filters, setFilters] = useState({page: 1, take: 10, query: undefined as undefined | string});

    function fetchClasses({query, take, page}: { page: number, take: number, query: undefined | string }) {
        dispatch(fetchCountClassifications(query));
        dispatch(fetchClassifications({
            skip: take * (page - 1),
            take: take,
            query: query
        }));
    }

    useEffect(() => {
        fetchClasses(filters);
    }, []);

    useEffect(() => {
        if (!isFetchClassifications) {
            return
        }
        dispatch(setIsFetchClassifications(false));
        fetchClasses(filters);
    }, [isFetchClassifications])

    function onChangeInput(e: ChangeEvent<HTMLInputElement>) {
        setFilters((prev) => ({...prev, page: 1, query: e.target.value}));
    }

    function onSubmit() {
        fetchClasses(filters)
    }

    function onChangePage(page: number) {
        setFilters((prev) => ({...prev, page: page}));
        fetchClasses({...filters, page: page})
    }

    return (
        <div className={classes.block}>
            <h2 className={classes.h2}>Классификации документов</h2>
            <div className={classes.background}>
                <div className={classes.content}>
                    <div className={classes.controls}>
                        <article className={classes.controlInputWrapper}>
                            <InputText className={classes.controlInput} onChange={onChangeInput}
                                       placeholder={"Поиск классификаций документов"}/>
                            <button className={classes.searchBtn} type="submit" onClick={onSubmit}><Search/>
                            </button>
                        </article>
                        <Button onClick={() => dispatch(openModal({type: "create"}))} className={classes.controlBtn}>
                            <PlusIcon className={classes.icon}/>Создать классификацию
                        </Button>
                    </div>
                    {classifications && classifications.length > 0 ? <ClassesItems classifications={classifications}/> :
                        <Empty notFound={classifications !== null}/>}
                </div>
                <Paginator pageHandler={onChangePage} current={filters.page} count={Math.ceil(count / filters.take)}/>
            </div>

        </div>
    );
};

const Empty: FC<{ notFound: boolean }> = ({notFound}) => {
    return <div className={classes.classesEmpty}>Классификации
        документов {!notFound ? "пока не созданы" : "не найдены"}</div>;
}


export default DocsClassesPage;
