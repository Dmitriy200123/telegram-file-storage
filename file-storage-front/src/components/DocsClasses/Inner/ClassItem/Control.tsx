import React, {FC, memo, useState} from "react";
import {OutsideAlerter} from "../../../utils/OutSideAlerter/OutSideAlerter";
import {ReactComponent as Edit} from "../../../../assets/edit.svg";
import {ReactComponent as Delete} from "../../../../assets/delete.svg";
import {ReactComponent as Rename} from "../../../../assets/rename.svg";
import classesItems from "../DocsClassesItems.module.scss";
import {classesDocsSlice} from "../../../../redux/classesDocs/classesDocsSlice";
import {useAppDispatch, useAppSelector} from "../../../../utils/hooks/reduxHooks";
import {Link} from "react-router-dom";
import {Rights} from "../../../../models/File";

const {openModal} = classesDocsSlice.actions;
type PropsControlType = {
    id:string,
    name:string,
};

export const Controls: FC<PropsControlType> = memo(({id, name}) => {
    const [isOpen, changeIsOpen] = useState(false);
    const dispatch = useAppDispatch();
    const {rights} = useAppSelector((state) => state.profile);
    const openModalRemove = () => {
        dispatch(openModal({type: "remove", args: {id: id}}));
    };
    const openModalRename = () => {
        dispatch(openModal({type: "edit", args: {id: id, name}}));
    }
    const openModalEdit = () => {
        //    todo: redirect
    }
    return <OutsideAlerter onOutsideClick={() => changeIsOpen(false)}>
         <div>
             {(rights?.includes(Rights["Редактирование классификаций"]) || rights?.includes(Rights["Удаление классификаций"])) && <div onClick={(e) => {
                e.preventDefault();
                changeIsOpen(true);
            }} className={classesItems.controls__btn}>
                <div className={classesItems.controls__circle}/>
            </div>}
            {isOpen && (rights?.includes(Rights["Редактирование классификаций"]) || rights?.includes(Rights["Удаление классификаций"])) && <section className={classesItems.controls__modal}>
                {rights?.includes(Rights["Редактирование классификаций"]) && <div className={classesItems.controls__modalItem}
                     onClick={openModalRename}>
                    <Rename/><span>Переименовать</span></div>}
                {rights?.includes(Rights["Редактирование классификаций"]) && <Link className={classesItems.controls__modalItem} to={`/docs-сlasses/${id}`}
                      onClick={openModalEdit}>
                    <Edit/><span>Редактировать</span></Link>}
                {rights?.includes(Rights["Удаление классификаций"]) && <div className={[classesItems.controls__modalItem, classesItems.controls__modalItem_delete].join(" ")}
                     onClick={openModalRemove}>
                    <Delete/><span>Удалить</span></div>}
            </section>
            }
        </div>
    </OutsideAlerter>
});

