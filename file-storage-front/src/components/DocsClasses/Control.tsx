import {filesSlice} from "../../redux/filesSlice";
import React, {FC, memo, useState} from "react";
import {OutsideAlerter} from "../utils/OutSideAlerter/OutSideAlerter";
import {ReactComponent as Edit} from "../../assets/edit.svg";
import {ReactComponent as Delete} from "../../assets/delete.svg";
import {Dispatch} from "@reduxjs/toolkit";
import classesItems from "./DocsClassesItems.module.scss";

const {openModal} = filesSlice.actions;
export const Controls: FC<PropsControlType> = memo(({
                                                        dispatch,
                                                    }) => {
    const [isOpen, changeIsOpen] = useState(false);
    const openModalRemove = () => {
        // () => dispatch(openModal({}));
    };
    const openModalRename = () => {
        // () => dispatch(openModal({}));
    }
    const openModalEdit = () => {
        // () => dispatch(openModal({}));
    }
    return <OutsideAlerter onOutsideClick={() => changeIsOpen(false)}>
        <div className={classesItems.controls}>
            <button onClick={(e) => {
                e.preventDefault();
                changeIsOpen(true);
            }} className={classesItems.controls__btn}>
                <div className={classesItems.controls__circle}/>
            </button>
            {isOpen && <section className={classesItems.controls__modal}>
                <div className={classesItems.controls__modalItem}
                     onClick={openModalEdit}>
                    <Edit/><span>Переименовать</span></div>
                <div className={classesItems.controls__modalItem}
                     onClick={openModalRename}>
                    <Edit/><span>Редактировать</span></div>
                <div className={[classesItems.controls__modalItem, classesItems.controls__modalItem_delete].join(" ")}
                     onClick={openModalRemove}>
                    <Delete/><span>Удалить</span></div>
            </section>
            }
        </div>
    </OutsideAlerter>
});

type PropsControlType = { dispatch: Dispatch<any> };
