import React, {FC} from 'react';
import classes from "./../DocsClassesModal/DocsClassesModal.module.scss";
import {InputText} from "../../../utils/Inputs/Text/InputText";
import {Button} from "../../../utils/Button/Button";

type Props = {
    onOutsideClick: () => void
}

const Create: FC<Props> = ({onOutsideClick}) => {
    return (
        <div className={classes.wrapper}>
            <h2 className={classes.h2}>Создание классификации документов</h2>
            <div className={classes.filters}>
                <div className={classes.filter}>
                    <label className={classes.label}>Название</label>
                    <InputText placeholder={"Введите название классификации"}></InputText>
                </div>
                <div className={[classes.filter, classes.filter_class].join(" ")}>
                    <label className={classes.label}>Слова для классификации</label>
                    <InputText placeholder={"Введите слово для классификации"}></InputText>
                    <div className={classes.tags}>
                        <div className={classes.tag}><span>Говно</span></div>
                        <div className={classes.tag}><span>Жопа</span></div>
                        <div className={classes.tag}><span>Помогите</span></div>
                        <div className={classes.tag}><span>Моргни</span></div>
                        <div className={classes.tag}><span>если</span></div>
                        <div className={classes.tag}><span>держат</span></div>
                        <div className={classes.tag}><span>в</span></div>
                        <div className={classes.tag}><span>заложниках</span></div>
                        <div className={classes.tag}><span>Говно</span></div>
                        <div className={classes.tag}><span>Жопа</span></div>
                        <div className={classes.tag}><span>Помогите</span></div>
                        <div className={classes.tag}><span>Моргни</span></div>
                        <div className={classes.tag}><span>если</span></div>
                        <div className={classes.tag}><span>держат</span></div>
                        <div className={classes.tag}><span>в</span></div>
                        <div className={classes.tag}><span>заложниках</span></div>
                        <button className={classes.tagsBtn}>Показать все</button>
                    </div>
                </div>
            </div>

            <div className={classes.btns}>
                <Button type={"transparent"} onClick={onOutsideClick}>ОТМЕНА</Button>
                <Button>ОК</Button>
            </div>
        </div>
    );
}

export default Create;
