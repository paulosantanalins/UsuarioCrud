import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';  
import { HttpHeaders } from '@angular/common/http';  
import { Observable } from 'rxjs';  
import { Usuario } from '../models/usuario';

var httpOptions = {headers: new HttpHeaders({"Content-Type": "application/json"})};

@Injectable({
  providedIn: 'root'
})
export class UsuarioService {

  url = 'http://localhost:17087/api/Usuario/';  

  constructor(private http: HttpClient) { }

  getTodosUsuarios(): Observable<Usuario[]> {  
    return this.http.get<Usuario[]>(this.url + "buscar-usuarios");  
  }  

  getUsuarioId(id:number) {  
    const apiurl = `${this.url}/buscar-por-id/${id}`;
    return this.http.get<Usuario>(apiurl);  
  }
  
  persistirUsuario(usuario: Usuario) {  
    return this.http.post<any>(`${this.url}`, usuario);  
  }
  
  deleteUsuario(alunoid: number) {  
    const apiurl = `${this.url}/${alunoid}`;
    return this.http.delete<number>(apiurl, httpOptions);  
  } 

  createAluno(aluno: Usuario): Observable<Usuario> {  
    return this.http.post<Usuario>(this.url, aluno, httpOptions);  
  }  

  atualizarUsuario(usuario: Usuario, id:number) {  
    //const apiurl = `${this.url}`, alunoid;
    return this.http.put<any>(`${this.url}` + id, usuario);  
    //return this.http.put<Usuario>(apiurl,aluno, httpOptions);  
  }  

  deleteAlunoById(alunoid: string): Observable<number> {  
    const apiurl = `${this.url}/${alunoid}`;
    return this.http.delete<number>(apiurl, httpOptions);  
  } 

}

