import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';  
import { Observable } from 'rxjs';  
import { Usuario } from '../models/usuario';
import { UsuarioService } from '../services/usuario.service';
import { DateAdapter } from '@angular/material';

@Component({
  selector: 'app-usuario',
  templateUrl: './usuario.component.html',
  styleUrls: ['./usuario.component.css']
})
export class UsuarioComponent implements OnInit {

  dataSaved = false;  
  usuarioForm: any;  

  usuario = new Usuario();

  allUsuarios: Observable<Usuario[]>; 

  message = null; 

  editar : boolean;
  idEditar : number;

  maxDate = new Date();
  

  constructor(private formbulider:FormBuilder, 
    private usuarioService:UsuarioService, private dateAdapter: DateAdapter<Date>) { 
      this.dateAdapter.setLocale('pt-BR');
    }

  ngOnInit() {
    this.usuarioForm = this.formbulider.group({  
      Nome: ['', [Validators.required]],  
      Email: ['', [Validators.required]],
      Sobrenome: ['', [Validators.required]],  
    });  
    this.carregarUsuarios();  
  }

  EndDateChange(event: any){
    this.usuario.dataNasc = event.value;
  }

  carregarUsuarios() {  
    this.allUsuarios = this.usuarioService.getTodosUsuarios();
  } 

  onFormSubmit() {  
    this.dataSaved = false;  
    this.usuarioForm.reset();  
  }

  changeEscolaridade(event: number){
    this.usuario.escolaridade = event;
  }

  validarDados(){
    if(this.usuario.dataNasc != null && this.usuario.escolaridade != null
      && this.usuario.nome != null && this.usuario.sobrenome != null && this.usuarioForm.get('Email').valid){
        return false;
      }else{
        return true;
      }
  }
  
  persistirUsuario(){
    if(this.idEditar){
      this.usuarioService.atualizarUsuario(this.usuario, this.idEditar).subscribe(() => {
        this.dataSaved = true;  
        this.message = 'Registro atualizado com sucesso';  
        this.carregarUsuarios(); 
      }, () => {
        this.dataSaved = false;  
        this.message = 'Falha na Atualização'; 
      });
      this.editar = false;
    }else{
      this.usuarioService.persistirUsuario(this.usuario).subscribe(() => {
        this.dataSaved = true;  
        this.message = 'Registro inserido com sucesso';  
        this.carregarUsuarios(); 
      }, () => {
        this.dataSaved = false;  
        this.message = 'Falha no cadastro'; 
      });
    }
    
  }

  deleteUsuario(id: number){

    this.usuarioService.deleteUsuario(id).subscribe(() => {
      this.message = 'Usuario Deletado com Sucesso';
      this.carregarUsuarios();
    }, () => {
      this.message = 'Falha em deletar'; 
    });

  }

  carregarUsuarioEditar(id: number){
    this.usuarioService.getUsuarioId(id).subscribe(data => {
      this.message = null;
      
      this.usuarioForm.controls['Nome'].setValue(data.nome);
      this.usuarioForm.controls['Email'].setValue(data.email);
      this.usuarioForm.controls['Sobrenome'].setValue(data.sobrenome);
      this.usuario.dataNasc = data.dataNasc;
      this.usuario.escolaridade = data.escolaridade;
      this.idEditar = id;
    })
  }

}
