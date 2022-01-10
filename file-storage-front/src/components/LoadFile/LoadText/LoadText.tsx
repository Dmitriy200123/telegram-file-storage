import React, {memo} from "react";
import {Dispatch} from "@reduxjs/toolkit";
import "./LoadText.scss"
import {Select} from "../../utils/Inputs/Select";
import {SubmitHandler, useForm} from "react-hook-form";
import {Button} from "../../utils/Button/Button";
import {optionsCategory} from "../../FilesMain/ConfigFilters";
import {Link} from "react-router-dom";
import {InputText} from "../../utils/Inputs/Text/InputText";

export const LoadText: React.FC<{ dispatch: Dispatch, className?: string }> = memo(({dispatch, className}) => {
    const {register, handleSubmit, formState: {errors}, setValue, getValues} = useForm();

    const dispatchValuesForm: SubmitHandler<any> = (formData) => {
        console.log(formData);
    };


    const onChangeForm = handleSubmit(dispatchValuesForm);
    const setValueForm = (name: any, value: any) => {
        setValue(name, value, {
            shouldValidate: true,
            shouldDirty: true
        });
    }

    return (
        <div className={className + "  load-text"}>
            <form onSubmit={onChangeForm} className={"load-text__form"}>
                <div className={"load-text__form-inputes"}>
                    <div>
                        <p>Тип Файла</p>
                        <Select name={"categories"} register={register}
                                onChangeForm={onChangeForm} setValue={setValueForm}
                                values={getValues("categories")} options={optionsCategory} isMulti={false}
                                placeholder={""}/>
                    </div>
                    <label>
                        <p>Название</p>
                        <InputText type={"text"} form={{...register("name")}}/>
                    </label>
                    <label>
                        <p>Ссылка или текст</p>
                        <InputText type={"text"} form={{...register("text")}}/>
                    </label>
                </div>
                <div className={"load-text__btns"}>
                    <Button>Загрузить</Button>
                    <Link to={"/load/"}><Button type={"transparent"}>Отмена</Button></Link>
                </div>
            </form>
        </div>

    );
})




