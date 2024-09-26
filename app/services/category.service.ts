// category.service.ts
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  url = environment.apiUrl;

  constructor(private httpClient: HttpClient) {}

  add(data: any) {
    return this.httpClient.post(this.url + '/category/addNewCategory', data, {
      headers: new HttpHeaders().set('Content-Type', 'application/json'),
    });
  }

  update(data: any) {
    return this.httpClient.post(this.url + '/category/updateCategory', data, {
      headers: new HttpHeaders().set('Content-Type', 'application/json'),
    });
  }

  getAllCategory() {
    return this.httpClient.get(this.url + '/category/getAllCategory');
  }
}
