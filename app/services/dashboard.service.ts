import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  url = environment.apiUrl;

  constructor(private htppClient: HttpClient) {}

  getDetails() {
    return this.htppClient.get(this.url + '/dashboard/details');
  }
}
