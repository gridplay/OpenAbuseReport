<?php
namespace App\Models;
use Illuminate\Database\Eloquent\Model;
use DB;
use Str;
class Reports extends Model {
	protected $table = 'abusereports';
	public $incrementing = false;
	public $timestamps = false;
	public static function savereport() {
		// $d = json_decode(file_get_contents('php://input'), true);
		$d = request()->only(['reporter', 'abuser', 'img', 'summary', 'details']);
		self::insert([
			'id' => Str::uuid(),
			'reporter' => $d['reporter'],
			'abuser' => $d['abuser'],
			'img' => $d['img'],
			'summary' => $d['summary'],
			'details' => $d['details'],
			'posted' => time()
		]);
	}
}