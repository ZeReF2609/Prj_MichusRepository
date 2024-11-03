const urlLocation = "../../controller/locationController.php";
const urlJoinUs = "../../controller/nominationController.php";
const appJoinUs = new Vue({
    el: "#appJoinUs",
    data: {
        stepActive: 1,

        data_department: [],
        data_province: [],
        data_locality: [],

        data: {
            form_name: '',
            form_type_doc: 'DNI',
            form_nro_doc: '',
            form_date_birth: '',
            form_nationality: 'Peruana',
            form_email: '',
            form_phone: '',
            form_gender: '',
            form_incapability: '',
            form_country_residence: 'Peru',
            form_department: 14,
            form_province: 129,
            form_locality: '',
            form_address: '',
            form_academic_grade: '',
            form_years_experience: '',
            form_expected_salary: '',
            form_availability: '',
            form_work_mode: '',
            form_application_area: '',
            form_cv: ''
        }
    },
    methods: {
        getDepartments() {
            axios.post(urlLocation,{option: 'getDepartment'}).then(res => {
                if (res.data.status == 1) {
                    this.data_department = res.data.data;
                    console.log(res.data.msg);
                }else {
                    alertify.error('Ha ocurrido un error');
                    console.log(res.data.msg);
                }
            });
        },
        getProvinceId() {
            this.data.form_province = '';
            this.data.form_locality = '';
            axios.post(urlLocation,{option: 'getProvinceId', idDepartment: this.data.form_department}).then(res => {
                if (res.data.status == 1) {
                    this.data_province = res.data.data;
                    console.log(res.data.msg);
                }else {
                    alertify.error('Ha ocurrido un error');
                    console.log(res.data.msg);
                }
            });
        },
        getLocalityId() {
            this.data.form_locality = '';
            axios.post(urlLocation,{option: 'getLocalityId', idProvince: this.data.form_province}).then(res => {
                if (res.data.status == 1) {
                    this.data_locality = res.data.data;
                    console.log(res.data.msg);
                }else {
                    alertify.error('Ha ocurrido un error');
                    console.log(res.data.msg);
                }
            });
        },
        submitForm(event) {
            let form = document.getElementById(event.target.id);
            if (!form.classList.contains('disabledForm')) form.classList.add("disabledForm");
            event.submitter.innerHTML = 'Enviando...';

            if (this.data.form_name == '' || this.data.form_type_doc == '' || this.data.form_nro_doc == '' || this.data.form_date_birth == '' ||
                this.data.form_nationality == '' || this.data.form_email == '' || this.data.form_phone == '' || this.data.form_gender == '' ||
                this.data.form_incapability == '' || this.data.form_country_residence == '' || this.data.form_department == '' || this.data.form_province == '' || this.data.form_locality == '' ||
                this.data.form_address == '' || this.data.form_academic_grade == '' || this.data.form_years_experience == '' || this.data.form_expected_salary == '' ||
                this.data.form_availability == '' || this.data.form_work_mode == '' || this.data.form_application_area == '') {
                    alertify.error('Por favor complete todos los campos');
            } else {
                let params = new FormData();
                params.append('option', 'submitNomination');
                params.append('name', this.data.form_name);
                params.append('type_doc', this.data.form_type_doc);
                params.append('nro_doc', this.data.form_nro_doc);
                params.append('date_birth', this.data.form_date_birth);
                params.append('nationality', this.data.form_nationality);
                params.append('email', this.data.form_email);
                params.append('phone', this.data.form_phone);
                params.append('gender', this.data.form_gender);
                params.append('incapability', this.data.form_incapability);
                params.append('country_residence', this.data.form_country_residence);
                params.append('iddepartment', this.data.form_department);
                params.append('idprovince', this.data.form_province);
                params.append('idlocality', this.data.form_locality);
                params.append('address', this.data.form_address);
                params.append('academic_grade', this.data.form_academic_grade);
                params.append('years_experience', this.data.form_years_experience);
                params.append('expected_salary', this.data.form_expected_salary);
                params.append('availability', this.data.form_availability);
                params.append('work_mode', this.data.form_work_mode);
                params.append('application_area', this.data.form_application_area);
                params.append('cv', this.data.form_cv);

                axios.post(urlJoinUs,params,{ headers: {'Content-Type': 'multipart/form-data'}}).then(res => {
                    if (res.data.status == 1) {
                        alertify.success('Postulación registrada exitosamente')
                        this.resetForm();
                    }else {
                        alertify.error('Ha ocurrido un error');
                        console.log(res.data.msg);
                    }

                    if (form.classList.contains('disabledForm')) form.classList.remove("disabledForm");
                    event.submitter.innerHTML = 'Enviar Postulación';
                }).catch((error) => {console.log(error);})
            }
        },
        handleFileUpload() {
            this.data.form_cv = this.$refs.form_cv.files[0];
        },
        resetFileUpload() {
            this.$refs.form_cv.value = null;
            this.data.form_cv = '';
        },
        resetForm() {
            this.data.form_name= '',
            this.data.form_type_doc= 'DNI',
            this.data.form_nro_doc= '',
            this.data.form_date_birth= '',
            this.data.form_nationality= '',
            this.data.form_email= '',
            this.data.form_phone= '',
            this.data.form_gender= '',
            this.data.form_incapability= '',
            this.data.form_country_residence= 'Peru',
            this.data.form_department= '',
            this.data.form_province= '',
            this.data.form_locality= '',
            this.data.form_address= '',
            this.data.form_academic_grade= '',
            this.data.form_years_experience= '',
            this.data.form_expected_salary= '',
            this.data.form_availability= '',
            this.data.form_work_mode= '',
            this.data.form_application_area= '',

            this.stepActive=1,
            this.resetFileUpload();

           
        
            this.data.form_department = 14;
            this.getProvinceId();
            this.data.form_province = 129
            this.getLocalityId();
        }


    },
    mounted() {
        this.getDepartments();
        this.getProvinceId();
        this.data.form_province = 129;
        this.getLocalityId();
    },
    computed: {
        isdisabledSteps() {
            let disabled = false;
            if (this.stepActive == 1) {
                (this.data.form_name == '') ? disabled = true  : '';    
                (this.data.form_type_doc == '') ? disabled = true  : '';   
                (this.data.form_nro_doc == '') ? disabled = true  : '';   
                (this.data.form_date_birth == '') ? disabled = true  : '';   
                (this.data.form_email == '') ? disabled = true  : '';   
                (this.data.form_phone == '') ? disabled = true  : '';   
                (this.data.form_gender == '') ? disabled = true  : '';   
                (this.data.form_phone == '') ? disabled = true  : '';   
                (this.data.form_phone == '') ? disabled = true  : '';   
                (this.data.form_incapability == '') ? disabled = true  : '';   

            }

            if (this.stepActive == 2) {
                (this.data.form_nationality == '') ? disabled = true  : '';   
                (this.data.form_country_residence == '') ? disabled = true  : '';   
                (this.data.form_province == '') ? disabled = true  : '';   
                (this.data.form_locality == '') ? disabled = true  : '';   
                (this.data.form_address == '') ? disabled = true  : '';   
            }

            if (this.stepActive == 3) {
                (this.data.form_academic_grade == '') ? disabled = true  : '';   
                (this.data.form_years_experience == '') ? disabled = true  : '';   
                (this.data.form_expected_salary == '') ? disabled = true  : '';   
                (this.data.form_availability == '') ? disabled = true  : '';   
                (this.data.form_work_mode == '') ? disabled = true  : '';  
                (this.data.form_application_area == '') ? disabled = true  : ''; 
                (this.data.form_cv == '') ? disabled = true  : ''; 
            }            
            return disabled;
        }
    }
});