using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;

namespace YobitTradingBot.Models
{
    public class TimerModule : IHttpModule
    {
        // Таймер
        static Timer timer;
        // Время через которое срабатывает таймер
        long interval = 30000; //30 секунд примерно
        // Объект в котором выполняются действия одним потоком
        static object synclock = new object();

        // Словарь хранит количество итераций для каждого пользователя
        Dictionary<int, int> counterIterations = new Dictionary<int, int>();

        // объект info
        // decimal_places: количество разрешенных знаков после запятой
        // min_price: минимальная разрешенная цена
        // max_price: максимальная разрешенная цена
        // min_amount: минимальное разрешенное количество для покупки или продажи
        // hidden: пара скрыта(0 или 1)
        // fee: комиссия пары
        static InfoModel info = GetInfoGeneral();

        // Функция получает объект info с биржи (нужна для получения данных о минимальной сумме для покупки монет)
        static public InfoModel GetInfoGeneral()
        {
            #region        ПОЛУЧАЕМ ОБЪЕКТ INFO

            // Создаем объект получающий данные с сервера
            WebClient client = new WebClient();
            // Подготавливаем объект для загрузки данных
            client.Encoding = Encoding.UTF8;
            // Получаем данные по всем торговым парам
            string jsonInfo = client.DownloadString("https://yobit.net/api/3/info");

            // СОЗДАЕМ ОБЪЕКТ ОЛИЦЕТВОРЯЮЩИЙ INFO 
            var tradingPairs = JsonConvert.DeserializeObject<InfoModel>(jsonInfo);
            // Возвращаем объект
            return tradingPairs;

            #endregion
        }

        // Функция заполняет и возвращает список всех рублевых торговых пар элементами небольшого размера
        static public List<string> GetTradingPairs()
        {

            #region         НАЗВАНИЯ ТОРГОВЫХ ПАР ДОБАВЛЯЕМ В GetTradingPairs()

            // Список названий торговых пар (каждый элемент менее 500 байт)
            List<string> tradingPairStrings = new List<string>();

            // Строковые переменные хранящие список торговых пар по которым мы хотим запрашивать информацию (рублевые пары)
            string tradingPairString1 = "nmc_rur-ppc_rur-cam_rur-cann_rur-uro_rur-opal_rur-via_rur-sdc_rur-start_rur-gsx_rur-vtc_rur-smbr_rur-etco_rur-xqn_rur-gml_rur-xpy_rur-dgms_rur-noo_rur-nav_rur-nkt_rur-eags_rur-dgd_rur-sys_rur-tes_rur-sbit_rur-pnd_rur-xsp_rur-xpc_rur-zeit_rur-moto_rur-ok_rur-spr_rur-find_rur-nka_rur-ccb_rur-con_rur-gp_rur-blu_rur-ozc_rur-stk_rur-2015_rur-am_rur-xbs_rur-vpn_rur-decr_rur-obs_rur-rice_rur-bsty_rur-gig_rur-oma_rur-icn_rur-dgore_rur-amber_rur";
            string tradingPairString2 = "geo_rur-dgb_rur-kobo_rur-ybc_rur-ktk_rur-cbx_rur-nvc_rur-sic_rur-xvg_rur-lts_rur-pen_rur-crime_rur-btcr_rur-boom_rur-gsm_rur-pty_rur-voya_rur-bitb_rur-xco_rur-pay_rur-bitz_rur-twist_rur-mtlmc3_rur-slfi_rur-stp_rur-nxe_rur-xau_rur-pxl_rur-aecc_rur-btcry_rur-planet_rur-fire_rur-anal_rur-ldoge_rur-tec_rur-zirk_rur-xdb_rur-tab_rur-ndoge_rur-crave_rur-icash_rur-256_rur-gift_rur-bbcc_rur-urc_rur-trick_rur-u_rur-cc_rur-metal_rur-clam_rur";
            string tradingPairString3 = "karma_rur-cry_rur-mrp_rur-tcx_rur-note_rur-rdd_rur-cyp_rur-giz_rur-xpro_rur-tron_rur-etrust_rur-fonz_rur-egg_rur-lea_rur-hzt_rur-p7c_rur-8bit_rur-tp1_rur-cf_rur-ctk_rur-ntrn_rur-cov_rur-lux_rur-drz_rur-007_rur-sigu_rur-qtz_rur-dox_rur-xnx_rur-xms_rur-gum_rur-tdfb_rur-bub_rur-unit_rur-grav_rur-epy_rur-goat_rur-pnc_rur-utle_rur-isl_rur-pkb_rur-vtn_rur-fsn_rur-mcar_rur-shell_rur-bit16_rur-tb_rur-m1_rur-krak_rur-ba_rur-ast_rur";
            string tradingPairString4 = "mrb_rur-bta_rur-esc_rur-pta_rur-dcc_rur-arb_rur-twerk_rur-eqm_rur-nice_rur-db_rur-tng_rur-cs_rur-skb_rur-genius_rur-sed_rur-pre_rur-32bit_rur-fade_rur-snrg_rur-rad_rur-psy_rur-vcoin_rur-moin_rur-greed_rur-exb_rur-option_rur-gluck_rur-dra_rur-seeds_rur-nanas_rur-bnbx_rur-arpa_rur-vapor_rur-dub_rur-fist_rur-eoc_rur-xce_rur-drkt_rur-transf_rur-ilm_rur-gen_rur-fury_rur-sen_rur-nodx_rur-hedg_rur-smsr_rur-bod_rur-gtfo_rur-tennet_rur";
            string tradingPairString5 = "blus_rur-xra_rur-ext_rur-spx_rur-gmcx_rur-vtx_rur-poly_rur-hxx_rur-spktr_rur-gene_rur-crps_rur-bam_rur-sjw_rur-spec_rur-ea_rur-tagr_rur-haze_rur-nuke_rur-2bacco_rur-cyc_rur-err_rur-sandg_rur-noc_rur-shrm_rur-asn_rur-grexit_rur-mdt_rur-nebu_rur-ge_rur-ibits_rur-bios_rur-curves_rur-axiom_rur-chip_rur-iec_rur-spc_rur-capt_rur-sak_rur-ros_rur-acp_rur-tam_rur-hifun_rur-nrc_rur-sql_rur-xfcx_rur-circ_rur-solo_rur-are_rur-beez_rur-netc_rur";
            string tradingPairString6 = "zoom_rur-pak_rur-peo_rur-strp_rur-deth_rur-ioc_rur-mystic_rur-aum_rur-dirt_rur-sdp_rur-l7s_rur-fcash_rur-tx_rur-ams_rur-mad_rur-gcc_rur-grf_rur-vol_rur-gsy_rur-heel_rur-rbt_rur-lhc_rur-click_rur-pure_rur-para_rur-n2o_rur-odnt_rur-power_rur-dcre_rur-two_rur-scrt_rur-lgbtq_rur-tur_rur-emp_rur-xmine_rur-vec_rur-nat_rur-btz_rur-cme_rur-space_rur-dogeth_rur-srnd_rur-swing_rur-bre_rur-boli_rur-caid_rur-sthr_rur-mapc_rur-fly_rur-bot_rur-phr_rur";
            string tradingPairString7 = "ltd_rur-scitw_rur-psi_rur-cyt_rur-xssx_rur-infx_rur-steps_rur-frdc_rur-bstk_rur-tia_rur-xup_rur-hsp_rur-sweet_rur-dtt_rur-nodes_rur-ban_rur-ronin_rur-fuzz_rur-atm_rur-xltcg_rur-cv2_rur-cb_rur-plnc_rur-maze_rur-bsh_rur-smc_rur-epc_rur-dem_rur-ivz_rur-zur_rur-xde_rur-rms_rur-os76_rur-adc_rur-cube_rur-dcyp_rur-gcr_rur-rep_rur-dct_rur-anti_rur-emc_rur-moneta_rur-lun_rur-bsc_rur-dec_rur-drop_rur-duo_rur-wbb_rur-ecli_rur-fre_rur-xmt_rur";
            string tradingPairString8 = "nubis_rur-1337_rur-dgcs_rur-mue_rur-bttf_rur-stv_rur-rubit_rur-lvg_rur-mm_rur-xsy_rur-pxi_rur-dft_rur-bst_rur-limx_rur-bdc_rur-evo_rur-veg_rur-rnc_rur-creva_rur-evil_rur-jpc_rur-x2_rur-ratio_rur-zet2_rur-dc_rur-av_rur-bucks_rur-bronz_rur-adz_rur-pnk_rur-biton_rur-mis_rur-ssc_rur-cd_rur-richx_rur-mnd_rur-egc_rur-c0c0_rur-kgc_rur-sxc_rur-bac_rur-ltcr_rur-sls_rur-html5_rur-btp_rur-opes_rur-strb_rur-buzz_rur-xde2_rur-jif_rur-nlg_rur";
            string tradingPairString9 = "btcu_rur-cpc_rur-frn_rur-kat_rur-spt_rur-lucky_rur-dust_rur-warp_rur-draco_rur-lkc_rur-moond_rur-drm_rur-pivx_rur-zet_rur-nkc_rur-brdd_rur-gbit_rur-sts_rur-hire_rur-six_rur-pex_rur-super_rur-credit_rur-npc_rur-dime_rur-ttc_rur-zmc_rur-frk_rur-crw_rur-tit_rur-cloak_rur-ffc_rur-club_rur-rbies_rur-tek_rur-dblk_rur-cyg_rur-dbic_rur-dcr_rur-spex_rur-trump_rur-bigup_rur-aclr_rur-val_rur-leaf_rur-xjo_rur-arco_rur-inc_rur-vip_rur-eurc_rur";
            string tradingPairString11 = "cfc_rur-bern_rur-hodl_rur-hmp_rur-neva_rur-payp_rur-unf_rur-altc_rur-pulse_rur-ims_rur-ixc_rur-pcm_rur-corg_rur-tbcx_rur-hawk_rur-xbu_rur-post_rur-888_rur-ene_rur-yac_rur-song_rur-aib_rur-elco_rur-loot_rur-czeco_rur-edrc_rur-jack_rur-adcn_rur-bill_rur-xav_rur-dxc_rur-mojo_rur-gbt_rur-coxst_rur-xng_rur-tcr_rur-pinkx_rur-blry_rur-orly_rur-tkn_rur-n7_rur-clint_rur-brain_rur-grow_rur-acid_rur-esp_rur-disk_rur-bomb_rur-trap_rur-sub_rur";
            string tradingPairString12 = "xbtc21_rur-op_rur-tak_rur-num_rur-gotx_rur-edcx_rur-chrg_rur-apt_rur-rcx_rur-ponzi_rur-xptx_rur-loc_rur-gre_rur-carbon_rur-cst_rur-tool_rur-tech_rur-acoin_rur-qbc_rur-ocow_rur-lth_rur-2give_rur-soul_rur-lana_rur-uae_rur-boss_rur-wmc_rur-spm_rur-bvc_rur-cmt_rur-uis_rur-sstc_rur-prm_rur-mxt_rur-ionx_rur-ghs_rur-eko_rur-b2_rur-aur_rur-ponz2_rur-mmxvi_rur-go_rur-euc_rur-eths_rur-empc_rur-clud_rur-cab_rur-glc_rur-gpu_rur-coin_rur-pwr_rur";
            string tradingPairString13 = "vprc_rur-sfe_rur-inv_rur-hcc_rur-dbtc_rur-btco_rur-tra_rur-ree_rur-mnm_rur-flvr_rur-dbg_rur-star_rur-kubo_rur-incp_rur-hvco_rur-gb_rur-exit_rur-ecchi_rur-bon_rur-mpro_rur-flx_rur-404_rur-shrek_rur-cox_rur-sta_rur-scan_rur-htc_rur-talk_rur-sling_rur-sp_rur-rio_rur-xpd_rur-enter_rur-csmic_rur-chess_rur-nic_rur-marv_rur-chemx_rur-alc_rur-dck_rur-putin_rur-lir_rur-token_rur-yay_rur-vene_rur-radi_rur-gt_rur-ctl_rur-air_rur-scrpt_rur";
            string tradingPairString14 = "ibank_rur-artc_rur-cj_rur-nzc_rur-taj_rur-xt_rur-craft_rur-rust_rur-frwc_rur-rpc_rur-poke_rur-gakh_rur-today_rur-tpg_rur-gain_rur-goon_rur-bpok_rur-ymc_rur-zne_rur-sct_rur-pal_rur-olymp_rur-px_rur-lc_rur-aces_rur-ccx_rur-crx_rur-way_rur-synx_rur-sh_rur-crnk_rur-b3_rur-xoc_rur-xhi_rur-bxt_rur-jwl_rur-fx_rur-btd_rur-vty_rur-sto_rur-vlt_rur-tot_rur-tell_rur-tao_rur-rcn_rur-mst_rur-pio_rur-zyd_rur-ubiq_rur-dlc_rur-ring_rur-arm_rur";
            string tradingPairString15 = "sport_rur-omc_rur-xbts_rur-ego_rur-xpo_rur-hallo_rur-dpay_rur-jobs_rur-psb_rur-bs_rur-crab_rur-forex_rur-team_rur-coc_rur-choof_rur-royal_rur-dkc_rur-equal_rur-wine_rur-jw_rur-ent_rur-beep_rur-wash_rur-rbit_rur-imps_rur-bash_rur-zec_rur-insane_rur-asafe_rur-zlq_rur-zecd_rur-liv_rur-iflt_rur-cbd_rur-nbit_rur-uxc_rur-lsd_rur-tmrw_rur-game_rur-emb_rur-365_rur-nlc2_rur-iw_rur-fgz_rur-boson_rur-acpr_rur-scash_rur-kurt_rur-xcre_rur-shorty_rur";
            string tradingPairString16 = "nodc_rur-icon_rur-coral_rur-arh_rur-money_rur-bstar_rur-look_rur-jane_rur-in_rur-vidz_rur-yes_rur-cg_rur-lenin_rur-aby_rur-jok_rur-bpc_rur-ride_rur-smf_rur-lepen_rur-cwxt_rur-posw_rur-prx_rur-icob_rur-nixon_rur-alex_rur-marx_rur-visio_rur-nef_rur-mcrn_rur-lizi_rur-tse_rur-wisc_rur-biob_rur-argus_rur-luna_rur-stalin_rur-mlite_rur-arcx_rur-units_rur-chat_rur-milo_rur-got_rur-volt_rur-mavro_rur-zeni_rur-mlnc_rur-mbit_rur-tlex_rur-cxt_rur";
            string tradingPairString17 = "conx_rur-nanox_rur-cjc_rur-vtl_rur-thom_rur-ccc_rur-all_rur-mcoin_rur-iti_rur-sev_rur-hpc_rur-arta_rur-scs_rur-best_rur-eco_rur-fazz_rur-kush_rur-blazr_rur-xve_rur-take_rur-stonk_rur-vntx_rur-usc_rur-ecn_rur-uni_rur-kids_rur-cnt_rur-frst_rur-xby_rur-al_rur-xvs_rur-cks_rur-c2_rur-mental_rur-dashs_rur-mvr_rur-dea_rur-proc_rur-may_rur-tle_rur-netko_rur-xin_rur-tide_rur-vec2_rur-cnnc_rur-bab_rur-glo_rur-eva_rur-dux_rur-party_rur-pupa_rur";
            string tradingPairString18 = "xps_rur-tap_rur-batl_rur-muu_rur-ctic_rur-alien_rur-bitok_rur-greenf_rur-mat_rur-mergec_rur-bits_rur-tcn_rur-witch_rur-lumi_rur-xtr_rur-cmc_rur-bhc_rur-kashh_rur-vers_rur-f16_rur-fjc_rur-pipr_rur-hams_rur-onx_rur-omg_rur-jnt_rur-skull_rur-xmg_rur-wok_rur-hon_rur-egame_rur-pie_rur-fidgt_rur-udown_rur-week_rur-knc_rur-max_rur-slco_rur-xnm_rur-kiss_rur-profit_rur-tag_rur-shrp_rur-elc_rur-rby_rur-qtm_rur-tlosh_rur-420g_rur-rbbt_rur";
            string tradingPairString19 = "solar_rur-hyperx_rur-ocean_rur-acrn_rur-lbtcx_rur-europe_rur-socc_rur-rocket_rur-mao_rur-cdo_rur-700_rur-women_rur-multi_rur-rh_rur-wam_rur-hmc_rur-fpc_rur-linda_rur-kgb_rur-rsgp_rur-rup_rur-dxo_rur-primu_rur-xgtc_rur-fidel_rur-dur_rur-ctic2_rur-ingt_rur-ger_rur-ant_rur-gsr_rur-wink_rur-poppy_rur-dota_rur-adam_rur-edit_rur-olit_rur-cin_rur-jocker_rur-wrt_rur-vrp_rur-funk_rur-bbh_rur-play_rur-ltcu_rur-usde_rur-rupx_rur-benji_rur-crm_rur";
            string tradingPairString20 = "sbt_rur-rid_rur-rai_rur-sleep_rur-karmc_rur-bcm_rur-agri_rur-sel_rur-chill_rur-shdw_rur-wcash_rur-ani_rur-wgo_rur-wgr_rur-vk_rur-sigt_rur-eqt_rur-lite_rur-ntm_rur-ruc_rur-hope_rur-cnx_rur-croc_rur-atb_rur-ixt_rur-ift_rur-dlt_rur-ind_rur-lunyr_rur-who_rur-pom_rur-bat_rur-eos_rur-gnt_rur-tnt_rur-time_rur-hac_rur-gno_rur-storj_rur-bnt_rur-scl_rur-kr_rur-exp_rur-des_rur-lsk_rur-waves_rur-rise_rur-dlisk_rur-etc_rur-bts_rur-ele_rur-laz_rur";
            string tradingPairString21 = "hmq_rur-mco_rur-snm_rur-ping_rur-plbt_rur-inpay_rur-xem_rur-ecob_rur-zrc_rur-mgo_rur-unify_rur-minh_rur-rec_rur-vulc_rur-altcom_rur-comp_rur-sw_rur-bcs_rur-agt_rur-gfl_rur-geld_rur-alis_rur-elite_rur-fit_rur-wic_rur-cct_rur-zonto_rur-mvc_rur-ldm_rur-lrc_rur-plu_rur-net_rur-vslice_rur-gup_rur-swt_rur-san_rur-nmr_rur-adt_rur-bgf_rur-tokc_rur-xios_rur-nyc_rur-btg_rur-kick_rur-soma_rur-onek_rur-atmcha_rur-btcred_rur-umc_rur-btcm_rur-drt_rur";
            string tradingPairString22 = "ieth_rur-ecash_rur-bio_rur-hdg_rur-dalc_rur-atl_rur-otn_rur-amm_rur-r_rur-yob2x_rur-xrl_rur-bcd_rur-etl_rur-dvd_rur-cag_rur-snc_rur-stu_rur-hkn_rur-ubtc_rur-bth_rur-sbtc_rur-tfl_rur-b2b_rur-trx_rur-wtt_rur-miro_rur-mntp_rur-lbtc_rur-b2x_rur-prs_rur-cms_rur-sdao_rur-req_rur-god_rur-bcp_rur-bum_rur-btcs_rur-pyn_rur-tbx_rur-tktx_rur-dbet_rur-cred_rur-prix_rur-bbt_rur-bca_rur-cat_rur-guess_rur-smart_rur-btv_rur-srnt_rur-enau_rur-inxt_rur";
            string tradingPairString23 = "eet_rur-occ_rur-chatx_rur-sent_rur-locx_rur-covx_rur-btca_rur-etz_rur-rntb_rur-sphtx_rur-smt_rur-arct_rur-sty_rur-wrc_rur-lhcoin_rur-bmt_rur-ntk_rur-crc_rur-fto_rur-pext_rur-clr_rur-wish_rur-tlx_rur-rea_rur-tds_rur-bm_rur-hps_rur-lcc_rur-taxi_rur-tgs_rur-ucash_rur-rac_rur-tie_rur-arna_rur-pac_rur-btdoll_rur-clo_rur-brh_rur-ing_rur-ukg_rur-dmt_rur-chp_rur-dtcn_rur-torq_rur-ltcp_rur-lif_rur-nbc_rur-bln_rur-latx_rur-faith_rur-poll_rur";
            string tradingPairString24 = "ust_rur-lion_rur-bptn_rur-chsb_rur-kin_rur-trnc_rur-mzi_rur-vvi_rur-cl_rur-storm_rur-emt_rur-ae_rur-lgr_rur-hqx_rur-brat_rur-qtg_rur-astr_rur-luc_rur-iqn_rur-ppt_rur-snt_rur-neu_rur-nbtk_rur-ufr_rur-mdz_rur-blue_rur-plc_rur-mart_rur-yrx_rur-vega_rur-lcwp_rur-rr_rur-echt_rur-bns_rur-bouts_rur-ssh_rur-tns_rur-mth_rur-arbit_rur-evn_rur-hand_rur-fntb_rur-ascs_rur-pkt_rur-ven_rur-btm_rur-zrx_rur-iost_rur-skrp_rur-wtl_rur-ert_rur-tyv_rur";
            string tradingPairString25 = "gold_rur-heal_rur-snpt_rur-dai_rur-get_rur-thug_rur-sat_rur-prex_rur-bcl_rur-flot_rur-mnz_rur-srn_rur-kbc_rur-wit_rur-loom_rur-tkln_rur-bnb_rur-cjt_rur-pmt_rur-lambo_rur-vrtm_rur-oko_rur-zipt_rur-pmnt_rur-aicyo_rur-lcd_rur-nbt_rur-lizun_rur-spd_rur-krl_rur-ml_rur-limbo_rur-abyss_rur-noah_rur-stud_rur-ink_rur-zdr_rur-rlt_rur-dtc_rur-lina_rur-molk_rur-sig_rur-cht_rur-buddy_rur-clt_rur-gmr_rur-bunny_rur-swm_rur-horse_rur-rct_rur";
            string tradingPairString26 = "ledu_rur-qws_rur-mtc_rur-sntr_rur-agr_rur-coinv_rur-beatx_rur-yotra_rur-tusd_rur-zil_rur-disc_rur-btrm_rur-gst_rur-ftec_rur-galax_rur-theta_rur-npxs_rur-wtc_rur-met_rur-ftm_rur-ren_rur-tfd_rur-pro_rur-hot_rur-pai_rur-qkc_rur-fc_rur-qash_rur-drgn_rur-man_rur-tomo_rur-tdh_rur-hur_rur-seth_rur-modx_rur-fund_rur-ht_rur-aoa_rur-nuls_rur-nexo_rur-homa_rur-pat_rur-ely_rur-vgr_rur-tvt_rur-work_rur-odem_rur-ttu_rur-fxt_rur-veri_rur-plr_rur";
            string tradingPairString27 = "swat_rur-gai_rur-cnc_rur-ubex_rur-meme_rur-robet_rur-link_rur-lky_rur-gvt_rur-stq_rur-ctxc_rur-sato_rur-wpr_rur-blz_rur-enj_rur-xyo_rur-pst_rur-yozi_rur-ccl_rur-ccup_rur-bgp_rur-exmr_rur-c20_rur-crpt_rur-lbr_rur-spank_rur-lend_rur-cnn_rur-rth_rur-box_rur-dadi_rur-mmt_rur-zip_rur-atx_rur-lba_rur-cdt_rur-dxt_rur-xrp_rur-secn_rur-snr_rur-yupa_rur-pax_rur-ukt_rur-sedo_rur-bchabc_rur-bchsv_rur-cbc_rur-usdc_rur-gusd_rur-eurs_rur-mxm_rur";
            string tradingPairString28 = "prg_rur-yobix_rur-kmx_rur-gpt_rur-ebsp_rur-ama_rur-bczero_rur-pma_rur-trix_rur-etcv_rur-dig_rur-mkr_rur-effm_rur-btt_rur-scam_rur-micro_rur-moon_rur-uranix_rur-pluto_rur-macro_rur-usd_rur-sib_rur-eth_rur-dash_rur-edc_rur-doge_rur-ltc_rur-btc_rur";

            // Добавляем к списку все элементы
            tradingPairStrings.Add(tradingPairString1);
            tradingPairStrings.Add(tradingPairString2);
            tradingPairStrings.Add(tradingPairString3);
            tradingPairStrings.Add(tradingPairString4);
            tradingPairStrings.Add(tradingPairString5);
            tradingPairStrings.Add(tradingPairString6);
            tradingPairStrings.Add(tradingPairString7);
            tradingPairStrings.Add(tradingPairString8);
            tradingPairStrings.Add(tradingPairString9);
            tradingPairStrings.Add(tradingPairString11);
            tradingPairStrings.Add(tradingPairString12);
            tradingPairStrings.Add(tradingPairString13);
            tradingPairStrings.Add(tradingPairString14);
            tradingPairStrings.Add(tradingPairString15);
            tradingPairStrings.Add(tradingPairString16);
            tradingPairStrings.Add(tradingPairString17);
            tradingPairStrings.Add(tradingPairString18);
            tradingPairStrings.Add(tradingPairString19);
            tradingPairStrings.Add(tradingPairString20);
            tradingPairStrings.Add(tradingPairString21);
            tradingPairStrings.Add(tradingPairString22);
            tradingPairStrings.Add(tradingPairString23);
            tradingPairStrings.Add(tradingPairString24);
            tradingPairStrings.Add(tradingPairString25);
            tradingPairStrings.Add(tradingPairString26);
            tradingPairStrings.Add(tradingPairString27);
            tradingPairStrings.Add(tradingPairString28);

            return tradingPairStrings;

            #endregion

        }

        // Функция принимает tradingPairString (строку хранящую нужные нам торговые пары)
        // Функция возвращает объект tradingPairs
        static public Dictionary<string, ParamsTradingPair> Ticker()
        {
            #region        ПОЛУЧАЕМ ОБЪЕКТ tradingPairs

            // ОБЪЕКТ ОБЛАДАЕТ СЛЕДУЮЩИМИ СВОЙСТВАМИ:
            // high: макcимальная цена
            // low: минимальная цена
            // avg: средняя цена
            // vol: объем торгов
            // vol_cur: объем торгов в валюте
            // last: цена последней сделки
            // buy: цена покупки (за какую цену продавать)
            // sell: цена продажи ( за какую цену мы будем покупать)
            // updated: последнее обновление кэша
            // range: выставляем в процессе ранжирования по формуле
            // min_amount : Минимальное кол во монеты которое можно купить (минимальны лот)

            // СОЗДАЕМ ОБЪЕКТ ОЛИЦЕТВОРЯЮЩИЙ TRADING PAIRS
            Dictionary<string, ParamsTradingPair> tradingPairsGeneralDictionary = new Dictionary<string, ParamsTradingPair>();

            foreach (var stringPairs in GetTradingPairs())
            {
                // Создаем объект получающий данные с сервера
                WebClient client = new WebClient();
                // Подготавливаем объект для загрузки данных
                client.Encoding = Encoding.UTF8;

                Start2:

                // Получаем данные по нужным торговым парам
                string jsonTicker = client.DownloadString("https://yobit.net/api/3/ticker/" + stringPairs);
                try
                {
                    // СОЗДАЕМ ОБЪЕКТ ОЛИЦЕТВОРЯЮЩИЙ TRADING PAIRS
                    Dictionary<string, ParamsTradingPair> tradingPairs = JsonConvert.DeserializeObject<TradingPairs>(jsonTicker);
                    // Приводим полученный объект к словарю для объединения
                    // Dictionary<string, ParamsTradingPair> tradingPairsDictionary = tradingPairs;
                    tradingPairsGeneralDictionary = tradingPairsGeneralDictionary.Union(tradingPairs).ToDictionary(x => x.Key, x => x.Value);
                }
                catch
                {
                    // Подождем 1/10 секунды
                    Thread.Sleep(100);
                    // Повторим запрос
                    goto Start2;
                }
                
            }
            // Возвращаем значение
            return tradingPairsGeneralDictionary;

            #endregion
        }

        // Функция инициализирующая данный модуль
        public void Init(HttpApplication app)
        {
            timer = new Timer(new TimerCallback(TradeOnExchange), null, 0, interval);
        }

        // В функцию передаем строковые переменные:
        // 1. Параметры запроса
        // 2. Секретная строка текущего пользователя
        // 3. Ключь текущего пользователя
        // ПОЛУЧАЕМ СТРОКУ С JSON СОДЕРЖИМЫМ ОТВЕТА В ЗАВИСИМОСТИ ОТ ПАРАМЕТРОВ
        public string GetRquest(string parameters, string secret, string key)
        {


            // адрес для трейд апи запросов
            string address = $"https://yobit.net/tapi//";



            // Строка с секретным ключом ЗАМЕНИТЬ НА ДАННЫЕ СЕКРЕТНОГО КЛЮЧA ПОЛЬЗОВАТЕЛЯ (конвертированная)
            var keyByte = Encoding.UTF8.GetBytes(secret);


            // Переменная подписи
            string sign1 = string.Empty;

            // Конвертируем строку с параметрами адресной строки в маасив байт
            byte[] inputBytes = Encoding.UTF8.GetBytes(parameters);



            using (var hmac = new HMACSHA512(keyByte))
            {
                // хешируем строку с параметрами адресной строки
                byte[] hashValue = hmac.ComputeHash(inputBytes);

                // Возвращем данные в строковый формат
                StringBuilder hex1 = new StringBuilder(hashValue.Length * 2);
                foreach (byte b in hashValue)
                {
                    hex1.AppendFormat("{0:x2}", b);
                }
                // помещаем данные подписи в ранее объявленную переменную с подписью
                sign1 = hex1.ToString();
            }

            // Создаем переменную запроса
            WebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(address);

                // Метод веб запроса
                webRequest.Method = "POST";
                // Время в течение которого мы ждем ответа от ресурса
                webRequest.Timeout = 20000;
                // При переопределении во вложенном классе возвращает или задает длину содержимого запрошенных к передаче данных.
                webRequest.ContentType = "application/x-www-form-urlencoded";
                // Добавляем к заголовку запроса данные ключа НУЖНО БУДЕТ ПЕРЕОПРЕДЕЛИТЬ НА ДАННЫЕ ПОЛЬЗОВАТЕЛЯ
                webRequest.Headers.Add("Key", key);
                // Добавляет данные подписи к заголовку запроса
                webRequest.Headers.Add("Sign", sign1);

                // Получаем длинну строки с параметрами
                webRequest.ContentLength = parameters.Length;


        // Для обработки частых исключений пришлось прибегруть к данному радикальному сценарию goto.
        // Если сервер вернет частую ошибку 500 начнем выполнение кода от сюда.
        Start:

                try
                {
                    // В данной области мы отправляем на сервер в виде потока наш сформированный запрос (записываем в поток)
                    using (var dataStream = webRequest.GetRequestStream())
                    {
                        dataStream.Write(inputBytes, 0, parameters.Length);
                    }

                    // После того как мы закончили отправку запроса на удаленный сервер биржи, мы записываем
                    // в области кода получаемый поток в переменную s.
                    // Далее мы создаем переменную читающую данные из потока sr (stream reader)
                    // В конечном итоге мы помещаем прочитанную строку в строковую переменную jsonResponse
                    // Отправляем запрос и ответ от сервера помещаем в переменную jsonResponse

                    using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            var jsonResponse = sr.ReadToEnd();

                        // Биржа Yobit не может обрабатывать запросы чаще 2 секунд.
                        // После того как выполняется какой либо запрос к бирже мы
                        // ждем 2 секунды
                        Thread.Sleep(2000);

                        return jsonResponse;
                        }
                    }
                }
                catch
                {
                    // Подождем 1/10 секунды
                    Thread.Sleep(100);
                    // Повторим запрос
                    goto Start;
                }

        }

        // Функция возвращает string с рублевыми торговыми парами торгуемыми на бирже
        static public String GetTradingPairString(InfoModel info)
        {
            String stringPairs = "";
            foreach (var a in info.pairs.Where(a => a.Key.Contains("rur")))
            { stringPairs = stringPairs + a.Key + "-"; }
            return stringPairs;
        }

        // ПЕРЕМЕННЫЕ КОТОРЫЕ НУЖНО БУДЕТ ОПРЕДЕЛИТЬ В БУДУЩЕМ В БД (как настроечные параметры)
        // Максимальная стоимость ордера в рублях
        double maxOrder = 10;
        // Скидка которую мы даем во время распродажи 
        double sellPriseDiscount = 0.001;




        // ***** Функция осуществляющая торговлю на бирже *****
        private void TradeOnExchange(object obj)
        {
            // Блокируем поток
            lock (synclock)
            {
                // В данном блоке using используем контекст базы данных
                using (var db = new ApplicationContext())
                {
                    // Перебираем всех ботов из БД которые включены
                    foreach (var uBot in db.BotModels.Where(b => b.IsOn == true).ToList())
                    {
                        // Хранит данные пользователя - владельца данного бота
                        var user = db.Users.First(u => u.Id == uBot.UserId);

                        // 1. ЗАПРАШИВАЕМ ДАННЫЕ ТЕКУЩИХ СЧЕТОВ 

                        #region        1.1 ПОЛУЧАЕМ ОБЪЕКТ objectGetInfo

                        // ОБЪЕКТ ОБЛАДАЕТ СЛЕДУЮЩИМИ СВОЙСТВАМИ:
                        // funds: баланс аккаунта, доступный к использованию (не включает деньги на открытых ордерах)
                        // funds_incl_orders: баланс аккаунта, доступный к использованию (включает деньги на открытых ордерах)
                        // rights: привилегии ключа. withdraw не используется (зарезервировано)
                        // transaction_count: всегда 0 (устарело)
                        // open_orders: всегда 0 (устарело)
                        // server_time: время сервера

                        Get_Info:

                        // Параметры в адресной строке для метода getInfo а так же для параметра nonce, 
                        string getInfoParameters = $"method=getInfo&nonce=" + (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

                        // ОБЪЕКТ ОЛИЦЕТВОРЯЮЩИЙ СУЩНОСТЬ GET INFO
                        GetInfo objectGetInfo = JsonConvert.DeserializeObject<GetInfo>(GetRquest(getInfoParameters, user.SecretAPI, user.PrivatAPI));

                        // Иногда objectGetInfo возвращает null, в таком случае мы повторяем запрос
                        if (objectGetInfo.@return == null)
                        {
                            // В таких ситуациях запросим объект повторно
                            goto Get_Info;
                        }

                        #endregion

                        #region        1.2 ПОЛУЧАЕМ ОБЪЕКТ objectActiveOrdersList (список объектов objectActiveOrders)

                        // ЭЛЕМЕНТЫ СПИСКА ОБЛАДАЮТ СЛЕДУЮЩИМИ СВОЙСТВАМИ:
                        // ключи массива: ID ордеров
                        // pair: пара(пример: ltc_btc)
                        // type: тип операции(пример: buy или sell)
                        // amount: осталось купить или продать
                        // rate: цена покупки или продажи
                        // timestamp_created: время создания ордера
                        // status: всегда 0(устарел)


                        List<ActiveOrders> objectActiveOrdersList = new List<ActiveOrders>();

                        // Для каждого элемента на балансе и на ордерах
                        foreach (var a in objectGetInfo.@return.funds_incl_orders)
                        {

                            #region         1.2.1 ПОЛУЧАЕМ СТРОКОВУЮ ПЕРЕМЕННУЮ coinOrder (хранит название пары, находящейся на ордере)

                            // Строка названием монеты которые находятся на ордерах
                            string coinOrder = "";

                            // Если Монета из списка всех монет не содержится в списке на счете значит она на ордере
                            if (!objectGetInfo.@return.funds.Keys.Contains(a.Key)) { coinOrder = a.Key + "_rur"; }
                            else
                            {
                                // Для каждого элемента на балансе проверяем что бы содержалось разное количество денег, так и вычисляем суммы что на ордерах
                                foreach (var b in objectGetInfo.@return.funds)
                                {
                                    if (a.Key == b.Key & a.Value != b.Value) { coinOrder = a.Key + "_rur"; }
                                }
                            }

                            #endregion

                            #region         1.2.2 ПОЛУЧАЕМ ОБЪЕКТ objectActiveOrders

                            // Параметры в адресной строке для метода ActiveOrders, помещаем так же строку с открытыми ордерами, а так же данные для параметра nonce, 
                            string activeOrdersParameters = $"method=ActiveOrders&pair=" + coinOrder + "&nonce=" + (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

                            // ОБЪЕКТ ОЛИЦЕТВОРЯЮЩИЙ СУЩНОСТЬ ACTIVE ORDERS
                            ActiveOrders objectActiveOrders = JsonConvert.DeserializeObject<ActiveOrders>(GetRquest(activeOrdersParameters, user.SecretAPI, user.PrivatAPI));

                            #endregion
                                        
                            // Если объект не равен нулю добавим его в список
                            if (objectActiveOrders.@return != null)
                            {
                                objectActiveOrdersList.Add(objectActiveOrders);
                            }
                        }

                        #endregion



                        // 2.     !!! РАСПРОДАЖА !!!
                        // -ЕСЛИ ЭТО ПЕРВЫЙ ЗАПРОС НА БИРЖУ
                        // -ЕСЛИ ЭТО ПОСЛЕДНИЙ ЗАПРОС НА БИРЖУ ДО НАЧАЛА РАСПРОДАЖИ
                                    
                        #region        2.1 ДОБАВЛЯЕМ БОТА К СЛОВАРЮ ИТЕРАЦИЙ counterIterations

                        // Если в словаре итераций пока нет данных по пользователю то создаем их
                        if (!counterIterations.ContainsKey(uBot.Id)) { counterIterations.Add(uBot.Id, 0); }

                        #endregion

                        #region        2.2 ВЫПОЛНЯЕМ CANCEL ORDER ДЛЯ КАЖДОГО ОБЪЕКТА objectActiveOrdersList ЗАТЕМ ПРОДАДЕМ ВСЕ ЧУТЬ НИЖЕ СТОИМОСТИ НА РЫНКЕ

                        // 1. Если количество итераций 0 или превышает заданное кол-во а также существуют открытые ордера - закрываем их
                        if (((counterIterations[uBot.Id] == 0) || ((counterIterations[uBot.Id] * interval) >= (uBot.TimeLose *60 *60*1000))) && objectActiveOrdersList.Count != 0)
                        {
                                                
                                    #region        2.2.1 ВЫПОЛНЯЕМ CancelOrder ( операция отмены всех активных ордеров)

                                    // Цикл для каждого объекта objectActiveOrders из списка
                                    foreach (var a in objectActiveOrdersList)
                                    {

                                        // Если есть активные ордера - выполняем их закрытие
                                        if (a.@return != null)
                                        { 
                                            // Цикл для каждого открытого ордера в объекте objectActiveOrders
                                            foreach (var b in a.@return.Keys)
                                            {
                                                // Параметры в адресной строке для метода CancelOrder, помещаем так же id открытого ордера, а так же данные для параметра nonce, 
                                                string cancelOrderParameters = $"method=CancelOrder&order_id=" + b + "&nonce=" + (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                                                        
                                                // ВЫПОЛНЯЕМ CANCEL ORDER
                                                GetRquest(cancelOrderParameters, user.SecretAPI, user.PrivatAPI);
                                            }
                                        }

                                    }

                                    #endregion

                                    #region        2.2.2 ПОЛУЧАЕМ ОБЪЕКТ tradingPairs (функция ticker) этот объект хранит большой список монет на бирже а так же их текущую стоимость

                                    // Объект tradingPairs
                                    Dictionary<string, ParamsTradingPair> tradingPairs = Ticker();

                                    #endregion

                                    #region        2.2.3 ВЫСТАВЛЯЕМ ОРДЕРА ЧУТЬ НИЖЕ ЦЕНЫ ПОСЛЕДНЕЙ СДЕЛКИ ДЛЯ КАЖДОЙ МОНЕТЫ НА НАШЕМ СЧЕТУ (кроме рублей)

                                    // Цикл для каждоЙ монеты на нашем счету
                                    foreach (var m in objectGetInfo.@return.funds_incl_orders.Where(m => (m.Key != "rur") && (m.Value > 0 )))
                                    {
                                        // Цена для распродажи для торговой пары, скидываем цену на процент скидки
                                        var discountPrise = tradingPairs.First(s => s.Key == (m.Key + "_rur")).Value.buy * sellPriseDiscount;
                                        var sellPrise = tradingPairs.First(s => s.Key == (m.Key + "_rur")).Value.buy - discountPrise;

                                        // Приводим количество к string
                                        string amount = ((decimal)m.Value).ToString("F12").Replace(',', '.');
                                        // Приводим цену к string
                                        string rate = ((decimal)sellPrise).ToString("F12").Replace(',', '.');

                                        // Параметры в адресной строке для метода Trade
                                        string TradeParameters = $"method=Trade&pair=" + m.Key +"_rur" + "&type=sell" + "&amount=" + amount + "&rate=" + rate + "&nonce=" + (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

                                        // ВЫПОЛНЯЕМ Trade
                                        GetRquest(TradeParameters, user.SecretAPI, user.PrivatAPI);
                                    }

                                    #endregion

                                               
                                    // После распродажи возвращаем обнуляем значение счетчика
                                    counterIterations[uBot.Id] = 1;
                                                
                                    // Возвращаемся в самое начало и выполняем алгоритм с новыми данными
                                    goto Get_Info;
                        }
                                    
                        #endregion



                        // 3. ТОРГОВЛЯ НА БИРЖЕ

                        #region        3.1 ЦИКЛ ДЛЯ ВЫБОРА И ТОРГОВЛИ КОНКРЕТНЫМИ МОНЕТАМИ ИЗ ТЕКУЩЕГО РАНЖИРОВАННОГО СПИСКА

                        // Если количество открытых ордеров не привышает
                        // количество заданное пользователем как максимальное
                        // Так же - если баланс рублей больше размера минимального ордера выполняем:
                        if ((objectActiveOrdersList.Count <= uBot.TradePairs) && (objectGetInfo.@return.funds.First(f => f.Key == "rur").Value >= uBot.MinOrder))
                        {

                            #region        3.1.1 ПОЛУЧАЕМ ОБЪЕКТ tradingPairs (функция ticker) этот объект хранит большой список монет на бирже а так же их текущую стоимость

                            // Объект tradingPairs
                            Dictionary<string, ParamsTradingPair> tradingPairs = Ticker();

                            #endregion

                            #region        3.1.2 РАНЖИРУЕМ tradingPairs и добавляем дополнительную информацию

                            // Для каждой монеты из объекта tradingPairs производим расчет значения rank
                            foreach (var a in tradingPairs)
                            {
                                // Формула для расчета ранга каждой монеты полученной с биржи
                                a.Value.rank = ((a.Value.sell - a.Value.buy) / a.Value.buy) * a.Value.vol;

                                // Добавляем поле со значением минимального разрешенного количества для покупки или продажи
                                a.Value.min_amount = info.pairs.First(m => m.Key == a.Key).Value.min_amount;
                            }

                            #endregion

                            #region        3.1.3 ДОБАВЛЯЕМ ПЕРЕМЕННЫЕ countActivePair И caledActivePairs

                            // Переменная хранит количество ордеров которое можно открыть
                            // Понадобится для того что бы постоянно не запрашивать новый список активных ордеров
                            int countActivePair = uBot.TradePairs - objectActiveOrdersList.Count;
                                        
                            // Переменная хранит список названий всех активных ордеров данного пользователя
                            List<string> caledActivePairs = new List<string>();
                            // Добавляем пустой элемент для избежания ошибок в будущей проверке
                            caledActivePairs.Add("full");
                            // Наполняем список названий активных ордеров
                            foreach (ActiveOrders orders in objectActiveOrdersList)
                            {
                                // Если есть ордера добавляем
                                if (orders.@return != null)
                                { 
                                    foreach (var order in orders.@return)
                                    {
                                        caledActivePairs.Add(order.Value.pair);
                                    }
                                }

                            }

                            #endregion

                            #region        3.1.4 ПЕРЕБИРАЕМ МОНЕТЫ КОТОРЫЕ НАМ НУЖНО КУПИТЬ И ПОКУПАЕМ

                            // Цикл для выстраивания монет которые мы можем купить в список по рангу от предпочтительного к менее предпочтительного для покупки
                            // Тут же мы покупаем и выставляем ордера       
                            foreach (var coin in tradingPairs.OrderByDescending(pair => pair.Value.rank))
                            {
                                if ((!caledActivePairs.Contains(coin.Key)) && (coin.Value.sell >= uBot.MinPriceTrade) && (coin.Value.min_amount * coin.Value.sell * 1.01 <= objectGetInfo.@return.funds.First(f => f.Key == "rur").Value) && (coin.Value.min_amount * coin.Value.sell * 1.01 <= maxOrder))
                                {

                                    // Если у нас денег больше чем ограничение которыое мы внесли на покупку монеты то купим на сумму этого ограничения иначе на все имеющиеся средства
                                    if (objectGetInfo.@return.funds.First(f => f.Key == "rur").Value >= maxOrder)
                                    {
                                            // Максимальный ордер делим на уже увеличенную цену покупки в 1.001
                                            // Получаем количество которое мы можем закупить
                                            var amountNum = maxOrder/(coin.Value.sell * 1.001);
                                            // Приводим количество к string
                                            string amount = ((decimal)amountNum).ToString("F12").Replace(',', '.'); 
                                            // Приводим цену для покупки к string (так же увеличиваем ее в 1.001 )
                                            string rate = ((decimal)(coin.Value.sell*1.001)).ToString("F12").Replace(',', '.');

                                            // Параметры в адресной строке для метода Trade
                                            string TradeParameters = $"method=Trade&pair=" + coin.Key + "&type=buy" + "&amount=" + amount + "&rate=" + rate + "&nonce=" + (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

                                            // ВЫПОЛНЯЕМ Trade
                                            GetRquest(TradeParameters, user.SecretAPI, user.PrivatAPI);
                                    }
                                    else
                                    {
                                            // Максимальный ордер делим на уже увеличенную цену покупки в 1.001
                                            // Получаем количество которое мы можем закупить
                                            var amountNum = objectGetInfo.@return.funds.First(f => f.Key == "rur").Value / (coin.Value.sell * 1.001);
                                            // Приводим количество к string
                                            string amount = ((decimal)amountNum).ToString("F12").Replace(',', '.'); 
                                            // Приводим цену для покупки к string (так же увеличиваем ее в 1.001 )
                                            string rate = ((decimal)(coin.Value.sell*1.001)).ToString("F12").Replace(',', '.');

                                            // Параметры в адресной строке для метода Trade
                                            string TradeParameters = $"method=Trade&pair=" + coin.Key + "&type=buy" + "&amount=" + amount + "&rate=" + rate + "&nonce=" + (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

                                            // ВЫПОЛНЯЕМ Trade
                                            GetRquest(TradeParameters, user.SecretAPI, user.PrivatAPI);
                                    }

                                    #region        3.1.4.1 ЕСЛИ МОНЕТА ПОСТУПИЛА НА БАЛАНС ВЫСТАВЛЯЕМ ОРДЕР С НАКРУТКОЙ
                                    int countr = 0;
                                    Balance:

                                    // Параметры в адресной строке для метода getInfo а так же для параметра nonce, 
                                    getInfoParameters = $"method=getInfo&nonce=" + (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

                                    // ОБЪЕКТ ОЛИЦЕТВОРЯЮЩИЙ СУЩНОСТЬ GET INFO
                                    objectGetInfo = JsonConvert.DeserializeObject<GetInfo>(GetRquest(getInfoParameters, user.SecretAPI, user.PrivatAPI));
                         
                                    // Иногда objectGetInfo возвращает null, в таком случае мы повторяем запрос
                                    if (objectGetInfo.@return == null && countr < 5)
                                    {
                                        countr++;
                                        // В таких ситуациях запросим объект повторно
                                        goto Balance;
                                    }

                                    // Если монета появилась - выставляем ордер. Если монета еще не появилась то повторяем запрос
                                    if (objectGetInfo.@return.funds.Keys.Contains(coin.Key.Remove(coin.Key.Length - 4, 4)) && countr < 5)
                                    {
                                            // Количество монеты на балансе
                                            var amountNum = objectGetInfo.@return.funds.First(c => c.Key == (coin.Key.Remove(coin.Key.Length - 4, 4))).Value;
                                            // Цена по которой будем продавать увеличиваем ее на сумму при покупке 1.001
                                            // Затем на процент который с нас имеет биржа за 2 операции 1.004
                                            // И наконец на процент выгоды задаваемый пользователем

                                            //Переменная процента надбавки
                                            Double procent = ((((double)uBot.MinProfit) / 100) + 1);

                                            var sellPrise = (((coin.Value.sell*1.001)*1.004)*procent);

                                            // Приводим количество к string
                                            string amount = ((decimal)amountNum).ToString("F12").Replace(',', '.');
                                            // Приводим цену к string
                                            string rate = ((decimal)sellPrise).ToString("F12").Replace(',', '.');

                                            // Параметры в адресной строке для метода Trade
                                            string TradeParameters = $"method=Trade&pair=" + coin.Key + "&type=sell" + "&amount=" + amount + "&rate=" + rate + "&nonce=" + (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

                                            // ВЫПОЛНЯЕМ Trade
                                            GetRquest(TradeParameters, user.SecretAPI, user.PrivatAPI);
                                    }
                                    else if(countr < 5)
                                    {
                                        countr++;
                                        goto Balance;
                                    }

                                    #endregion

                                    #region        3.1.4.2 ЗАВЕРШЕНИЕ ТОРГОВОЙ ОПЕРАЦИИ С МОНЕТОЙ

                                    // Сокращаем количество возможных ордеров
                                    countActivePair--;

                                    // Добавляем к списку наших активных ордеров только что открытый
                                    caledActivePairs.Add(coin.Key);

                                    if (countActivePair == 0)
                                    {
                                        // Выходим из цикла foreach
                                        break;
                                    }
                                }

                                #endregion

                            }

                            #endregion

                        }

                        #endregion

                    }
                }
            }
        }



        public void Dispose()
        { }
    }
}